namespace Application.Common.Services.IdentityManager;

public class ResetPasswordResultDto
{
    public string Email { get; set; } = string.Empty;
    public bool PasswordReset { get; set; }
}
