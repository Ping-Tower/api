namespace Application.Common.Services.IdentityManager;

public class LoginResultDto
{
    public string UserName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
