namespace Application.Common.Services.IdentityManager;

public class VerifyEmailResultDto
{
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
}
