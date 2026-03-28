using Application.Common.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Infrastructure.IdentityManager.Tokens;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;
using Domain.IdentityManager.Security;
using Application.Common.Services.IdentityManager;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly AppDbContext _dbContext;
    private readonly TokenSettings _tokenSettings;
    private readonly IdentitySettings _identitySettings;
    private readonly IEmailService _emailService;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        AppDbContext dbContext,
        TokenSettings tokenSettings,
        IdentitySettings identitySettings,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _tokenSettings = tokenSettings;
        _identitySettings = identitySettings;
        _emailService = emailService;
    }
    public async Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new UnauthorizationException("Invalid login attempt.");

        if (user.IsBlocked)
            throw new UnauthorizationException("User is blocked.");

        if (_identitySettings.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
            throw new UnauthorizationException("Email is not confirmed.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (result.IsLockedOut)
            throw new UnauthorizationException("User is locked out.");

        if (!result.Succeeded)
            throw new UnauthorizationException("Invalid login attempt.");

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
            throw new UnauthorizationException(string.Join(",", result.Errors.Select(w => w.Description)));
        }

        await _userManager.AddToRoleAsync(user, AppRoles.User);

        if (!user.EmailConfirmed)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var welcomeStr = $"Dear {name}, Welcome to PingTower! Verification code: {encodedCode}";
            var subject = "Registration on PingTower";
            await _emailService.SendMessage(user.Email, subject, welcomeStr, cancellationToken);
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

    public async Task<string> VerifyEmail(string email, string code, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new UnauthorizationException("Invalid verify attempt.");

        var decodeCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, decodeCode);

        if (!result.Succeeded)
            throw new UnauthorizationException("Invalid verification code.");

        return email;
    }

    public async Task<string> ForgotPassword(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return "If email exists, reset instructions were sent.";

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var callbackUrl = $"https://pingtower.com/reset-password?email={email}&code={encodedToken}"; //FIX THIS!!!!!!

        await _emailService.SendMessage(
        email,
        "Reset Password",
        $"Reset your password by clicking <a href='{callbackUrl}'>here</a>",
        cancellationToken);

        return "If email exists, reset instructions were sent.";
    }

    public async Task<string> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new UnauthorizationException("Invalid reset attempt.");

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ResetPasswordAsync(user, code, newPassword);

        if (!result.Succeeded)
            throw new UnauthorizationException("Invalid login attempt.");

        return email;
    }

    public async Task<RefreshResultDto> Refresh(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _dbContext.Tokens.FirstOrDefaultAsync(w => w.RefreshToken == refreshToken, cancellationToken);

        if (token is null || token.IsDeleted)
            throw new UnauthorizationException("Token not found!");

        if (token.Expire < DateTime.UtcNow)
            throw new UnauthorizationException("Token already expire!");

        token.IsDeleted = true;

        var user = await _userManager.FindByIdAsync(token!.UserId!);

        if (user is null)
            throw new UnauthorizationException("User not found!");

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

    public async Task<string> ResetVerifyEmailCode(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is null)
            return "if email exists code sent";

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var decodeCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var welcomeStr = $"Dear {user.UserName}, Welcome to PingTower! Verification code: {decodeCode}";
        var subject = "Registration on PingTower";
        await _emailService.SendMessage(email, subject, welcomeStr, cancellationToken);

        return "if email exists code sent";
    }

    public async Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new UnauthorizationException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles
        };
    }

}