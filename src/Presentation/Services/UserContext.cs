using System.Security.Claims;
using Application.Common.Interfaces;

namespace Presentation.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;
    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public UserContext(IHttpContextAccessor accessor) => _accessor = accessor;

    public string? UserName => User?.FindFirst(ClaimTypes.Name)?.Value;
    public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public List<string>? Roles
    {
        get
        {
            var role = User?.FindAll(ClaimTypes.Role);
            return role?.Select(r => r.Value).ToList();
        }
    }
}