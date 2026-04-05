namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class AuthLinkSettings
{
    public string WebAppUrl { get; init; } = null!;
    public string VerifyEmailPathTemplate { get; init; } = "/auth/verify-email?email={email}&code={code}";
    public string ResetPasswordPathTemplate { get; init; } = "/auth/reset-password?email={email}&code={code}";

    public string BuildVerifyEmailUrl(string email, string code) =>
        BuildUrl(VerifyEmailPathTemplate, email, code);

    public string BuildResetPasswordUrl(string email, string code) =>
        BuildUrl(ResetPasswordPathTemplate, email, code);

    private string BuildUrl(string pathTemplate, string email, string code)
    {
        var path = pathTemplate
            .Replace("{email}", Uri.EscapeDataString(email), StringComparison.Ordinal)
            .Replace("{code}", Uri.EscapeDataString(code), StringComparison.Ordinal)
            .Trim();

        if (!path.StartsWith('/'))
            path = $"/{path}";

        return $"{WebAppUrl.TrimEnd('/')}{path}";
    }
}
