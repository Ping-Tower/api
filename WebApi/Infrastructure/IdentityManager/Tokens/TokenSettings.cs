namespace Infrastructure.IdentityManager.Tokens;

public class TokenSettings
{
    public string SecretKey { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public int ExpireInMinute { get; init; }
}