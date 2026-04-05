using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Infrastructure.IdentityManager.Tokens;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Security;
using Application.Common.Services.IdentityManager;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using MediatR;
using FluentValidation.Results;
using Infrastructure.RedisManager;
using StackExchange.Redis;

namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly AppDbContext _dbContext;
    private readonly TokenSettings _tokenSettings;
    private readonly IdentitySettings _identitySettings;
    private readonly AuthLinkSettings _authLinkSettings;
    private readonly AuthEmailRateLimitSettings _authEmailRateLimitSettings;
    private readonly IEmailService _emailService;
    private readonly IDatabase _redisDatabase;
    private readonly string _redisKeyPrefix;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        AppDbContext dbContext,
        TokenSettings tokenSettings,
        IdentitySettings identitySettings,
        AuthLinkSettings authLinkSettings,
        AuthEmailRateLimitSettings authEmailRateLimitSettings,
        RedisSettings redisSettings,
        IConnectionMultiplexer connectionMultiplexer,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _tokenSettings = tokenSettings;
        _identitySettings = identitySettings;
        _authLinkSettings = authLinkSettings;
        _authEmailRateLimitSettings = authEmailRateLimitSettings;
        _redisKeyPrefix = redisSettings.KeyPrefix;
        _redisDatabase = connectionMultiplexer.GetDatabase();
        _emailService = emailService;
    }
    public async Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedException("Invalid login attempt.");

        if (user.IsBlocked)
            throw new UnauthorizedException("User is blocked.");

        if (_identitySettings.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
            throw new UnauthorizedException("Email is not confirmed.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
        if (result.IsLockedOut)
            throw new UnauthorizedException("User is locked out.");

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid login attempt.");

        var (jwtToken, expire) = await _tokenService.GenerateJwtTokenAsync(user, cancellationToken);
        var refreshToken = _tokenService.GetRefreshToken();

        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            Expire = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireInDays)
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResultDto
        {
            UserName = user.UserName ?? string.Empty,
            UserId = user.Id,
            Token = jwtToken,
            RefreshToken = refreshToken,
            Expiration = expire
        };
    }

    public async Task<RegistrationResultDto> RegistrationAsync(
        string email,
        string password,
        string name,
        CancellationToken cancellationToken)
    {

        var user = new ApplicationUser
        {
            Email = email,
            UserName = name,
            EmailConfirmed = !_identitySettings.SignIn.RequireConfirmedEmail
        };

        var result = await _userManager.CreateAsync(user, password);


        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(error =>
                new ValidationFailure(error.Code, error.Description)));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            throw new UnauthorizedException(string.Join(",", roleResult.Errors.Select(w => w.Description)));
        }

        if (!user.EmailConfirmed)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await SendVerificationEmailAsync(
                user.Email ?? email,
                name,
                encodedCode,
                cancellationToken);
        }

        return new RegistrationResultDto
        {
            UserName = user.UserName ?? string.Empty,
            UserId = user.Id
        };
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken, cancellationToken);

        if (token is null)
            return;

        _dbContext.Tokens.Remove(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return;
    }

    public async Task<VerifyEmailResultDto> VerifyEmail(string email, string code, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new UnauthorizedException("Invalid verification attempt.");

        string decodeCode;
        try
        {
            decodeCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (Exception)
        {
            throw new UnauthorizedException("Invalid verification code.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, decodeCode);

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid verification code.");

        return new VerifyEmailResultDto
        {
            Email = user.Email ?? email,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public async Task<ForgotPasswordResultDto> ForgotPassword(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return new ForgotPasswordResultDto
            {
                Email = email,
                ResetRequested = true
            };
        }

        if (_identitySettings.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            return new ForgotPasswordResultDto
            {
                Email = email,
                ResetRequested = true
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        await SendPasswordResetEmailAsync(
            email,
            user.UserName ?? email,
            encodedToken,
            cancellationToken);

        return new ForgotPasswordResultDto
        {
            Email = email,
            ResetRequested = true
        };
    }

    public async Task<ResetPasswordResultDto> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new UnauthorizedException("Invalid reset attempt.");

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (Exception)
        {
            throw new UnauthorizedException("Invalid reset attempt.");
        }

        var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

        if (!result.Succeeded)
            throw new ValidationException(result.Errors.Select(error =>
                new ValidationFailure(error.Code, error.Description)));

        return new ResetPasswordResultDto
        {
            Email = user.Email ?? email,
            PasswordReset = true
        };
    }

    public async Task<RefreshResultDto> Refresh(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _dbContext.Tokens.FirstOrDefaultAsync(w => w.RefreshToken == refreshToken, cancellationToken);

        if (token is null || token.IsDeleted)
            throw new UnauthorizedException("Token not found.");

        if (token.Expire < DateTime.UtcNow)
            throw new UnauthorizedException("Token has already expired.");

        token.IsDeleted = true;

        var user = await _userManager.FindByIdAsync(token!.UserId!);

        if (user is null)
            throw new UnauthorizedException("User not found.");

        if (user.IsBlocked)
            throw new UnauthorizedException("User is blocked.");

        if (_identitySettings.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
            throw new UnauthorizedException("Email is not confirmed.");

        var (jwtToken, expire) = await _tokenService.GenerateJwtTokenAsync(user, cancellationToken);
        var newRefreshToken = _tokenService.GetRefreshToken();

        await _dbContext.Tokens.AddAsync(new Token
        {
            RefreshToken = newRefreshToken,
            UserId = user.Id,
            Expire = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireInDays)
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshResultDto
        {
            UserName = user.UserName ?? "",
            UserId = user.Id,
            Token = jwtToken,
            RefreshToken = newRefreshToken,
            Expiration = expire
        };
    }

    public async Task<ResendVerificationCodeResultDto> ResetVerifyEmailCode(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is null)
        {
            return new ResendVerificationCodeResultDto
            {
                Email = email,
                VerificationCodeSent = true
            };
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        await SendVerificationEmailAsync(
            email,
            user.UserName ?? email,
            encodedCode,
            cancellationToken);

        return new ResendVerificationCodeResultDto
        {
            Email = email,
            VerificationCodeSent = true
        };
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new UnauthorizedException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles
        };
    }

    private async Task SendVerificationEmailAsync(
        string email,
        string userName,
        string encodedCode,
        CancellationToken cancellationToken)
    {
        var key = RateLimitKey("verification-email", email);
        var ttl = TimeSpan.FromSeconds(_authEmailRateLimitSettings.VerificationEmailCooldownSeconds);

        await ExecuteRateLimitedEmailSendAsync(
            key,
            ttl,
            "Verification email was requested too frequently. Please try again later.",
            () => _emailService.SendMessageAsync(
                email,
                "registration-confirmation",
                new Dictionary<string, string?>
                {
                    ["userName"] = userName,
                    ["verificationCode"] = encodedCode,
                    ["verificationUrl"] = _authLinkSettings.BuildVerifyEmailUrl(email, encodedCode)
                },
                cancellationToken));
    }

    private async Task SendPasswordResetEmailAsync(
        string email,
        string recipient,
        string encodedToken,
        CancellationToken cancellationToken)
    {
        var key = RateLimitKey("password-reset-email", email);
        var ttl = TimeSpan.FromSeconds(_authEmailRateLimitSettings.PasswordResetEmailCooldownSeconds);

        await ExecuteRateLimitedEmailSendAsync(
            key,
            ttl,
            "Password reset email was requested too frequently. Please try again later.",
            () => _emailService.SendMessageAsync(
                email,
                "password-reset",
                new Dictionary<string, string?>
                {
                    ["recipient"] = recipient,
                    ["resetUrl"] = _authLinkSettings.BuildResetPasswordUrl(email, encodedToken)
                },
                cancellationToken));
    }

    private async Task ExecuteRateLimitedEmailSendAsync(
        string key,
        TimeSpan ttl,
        string errorMessage,
        Func<Task> sendEmail)
    {
        if (ttl <= TimeSpan.Zero)
        {
            await sendEmail();
            return;
        }

        var reserved = await _redisDatabase.StringSetAsync(
            key,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ttl,
            when: When.NotExists);

        if (!reserved)
            throw new TooManyRequestsException(errorMessage);

        try
        {
            await sendEmail();
        }
        catch
        {
            await _redisDatabase.KeyDeleteAsync(key);
            throw;
        }
    }

    private string RateLimitKey(string flow, string email) =>
        $"{_redisKeyPrefix}:rate-limit:auth:{flow}:{NormalizeEmail(email)}";

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

}
