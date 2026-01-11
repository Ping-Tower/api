using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Infrastructure.IdentityManager.AspNetCoreIdentity;


namespace Infrastructure.IdentityManager.Tokens;

public class TokenService
{
    private readonly TokenSettings _tokenSettings;
    private readonly ClaimService _claimService;

    public TokenService(IOptions<TokenSettings> options, ClaimService claimService)
    {
        _tokenSettings = options.Value;
        _claimService = claimService;
    }

    public async Task<(string, DateTime)> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var claims = await _claimService.GetClaimsAsync(user, cancellationToken);

        var key = GetSymmetricSecurityKey();
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(_tokenSettings.ExpireInMinute)
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwtToken), DateTime.Now.AddMinutes(_tokenSettings.ExpireInMinute));
    }

    private SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        var keyBytes = Encoding.UTF8.GetBytes(_tokenSettings.SecretKey);
        return new SymmetricSecurityKey(keyBytes);
    }

    public string GetRefreshToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
}