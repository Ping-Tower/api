namespace Infrastructure.IdentityManager.Tokens;

public class TokenSettings
{
    public string SecretKey { get; init; } = "CHANGE_ME_WITH_A_LONG_RANDOM_SECRET_KEY_AT_LEAST_32_CHARS";
    public string Audience { get; init; } = "PingTowerClient";
    public string Issuer { get; init; } = "PingTowerApi";
    public int ExpireInMinute { get; init; } = 60;
    public int RefreshTokenExpireInDays { get; init; } = 7;
}
