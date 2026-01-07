using Application.Common.Interfaces;
using Application.DTOs;
using Infrastructure.Identity.AspNetCoreIdentity;
using Microsoft.AspNetCore.Identity;
using Application.Common.Exceptions;
using Infrastructure.IdentityManager.Tokens;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;

namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly AppDbContext _dbContext;
    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService, AppDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
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

        var (jwtToken, expires) = await _tokenService.GenerateJwtToken(user, cancellationToken);
        var refreshToken = _tokenService.GetRefreshToken();

        _dbContext.Tokens.Where(t => t.UserId == user.Id).ToList().ForEach(t => _dbContext.Tokens.Remove(t));

        var token = new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            Expire = DateTime.UtcNow.AddDays(7)
        };

        await _dbContext.Tokens.AddAsync(token, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResultDto
        {
            UserName = user.UserName ?? string.Empty,
            UserId = user.Id,
            Token = jwtToken,
            RefreshToken = refreshToken,
            Expiration = expires
        };
    }

    public async Task<> LogoutAsync(string userId, CancellationToken cancellationToken)
    {
        var tokens = _dbContext.Tokens.Where(t => t.UserId == userId);
        _dbContext.Tokens.RemoveRange(tokens);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}