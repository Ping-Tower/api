namespace Application.Common.Services.IdentityManager;

public class ResendVerificationCodeResultDto
{
    public string Email { get; set; } = string.Empty;
    public bool VerificationCodeSent { get; set; }
}
