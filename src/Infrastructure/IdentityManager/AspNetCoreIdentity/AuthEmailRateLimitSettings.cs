namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class AuthEmailRateLimitSettings
{
    public int VerificationEmailCooldownSeconds { get; set; } = 60;
    public int PasswordResetEmailCooldownSeconds { get; set; } = 60;
}
