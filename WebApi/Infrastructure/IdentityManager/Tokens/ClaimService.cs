using System.Security.Claims;
using Infrastructure.DataManager.Contexts;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.IdentityManager.Tokens;
using Infrastructure.IdentityManager.AspNetCoreIdentity;

public class ClaimService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    public ClaimService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<List<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        if(user?.Id == null)
        {
            return new List<Claim>();
        }

        var userFromDb = await _dbContext.Users.FindAsync(user.Id, cancellationToken);

        if(userFromDb == null)
        {
            return new List<Claim>();
        }
            
        var claims = new List<Claim>
        {
           new Claim(ClaimTypes.NameIdentifier, userFromDb.Id),
           new Claim(ClaimTypes.Name, userFromDb.UserName ?? string.Empty),
           new Claim(ClaimTypes.Email, userFromDb.Email ?? string.Empty)
        };

        var roles = await _userManager.GetRolesAsync(userFromDb);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }   
}