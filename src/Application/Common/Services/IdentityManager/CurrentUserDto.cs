namespace Application.Common.Services.IdentityManager;

public class CurrentUserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public IList<string> Roles { get; set; } = [];
}
