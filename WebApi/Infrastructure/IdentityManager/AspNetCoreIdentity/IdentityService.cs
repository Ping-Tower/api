using Application.Common.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Infrastructure.IdentityManager.Tokens;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;
using Domain.IdentityManager.Security;

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
        {
            throw new UnauthorizationException("User not found.");
        }

        if(user.IsBlocked)
        {
            throw new UnauthorizationException("User is blocked.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if(result.IsLockedOut)
        {
            throw new UnauthorizationException("User is locked out.");
        }

        if (!result.Succeeded)
        {
            throw new UnauthorizationException("Invalid login attempt.");
        }

        var (jwtToken, expire) = await _tokenService.GenerateJwtToken(user, cancellationToken);
        var refreshToken = _tokenService.GetRefreshToken();

        await _dbContext.Tokens.AddAsync(new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            Expire = DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpireInDays)
        }, cancellationToken);
        
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
            Email=email,
            UserName=name,
            EmailConfirmed=!_identitySettings.SignIn.RequireConfirmedEmail
        };

        var result = await _userManager.CreateAsync(user, password);


        if(!result.Succeeded)
        {
            throw new UnauthorizationException(string.Join(",", result.Errors.Select(w => w.Description)));
        }

        await _userManager.AddToRoleAsync(user, AppRoles.User);

        if(!user.EmailConfirmed)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var welcomeStr = $"Dear {name}, Welcome to PingTower! Verification code: {code}";
            var subject = "Registration on PingTower";
            await _emailService.SendMessage(user.Email, subject, welcomeStr, cancellationToken);
        }
        
        return new RegistrationResultDto
        {
            UserName = user.UserName ?? string.Empty,
            UserId = user.Id
        };
    }

    public async Task<LogOutResultDto> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);

        if(token is null)
            return new LogOutResultDto();

        _dbContext.Tokens.Remove(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new LogOutResultDto();
    }
}