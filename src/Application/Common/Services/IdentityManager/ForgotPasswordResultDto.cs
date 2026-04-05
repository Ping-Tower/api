namespace Application.Common.Services.IdentityManager;

public class ForgotPasswordResultDto
{
    public string Email { get; set; } = string.Empty;
    public bool ResetRequested { get; set; }
}
