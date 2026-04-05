namespace Infrastructure.NotificationManager;

public class AppLinksSettings
{
    public string WebAppUrl { get; init; } = null!;
    public string ServerPathTemplate { get; init; } = "/servers/{serverId}";
    public string TelegramDashboardButtonText { get; init; } = "Dashboard";

    public string BuildServerUrl(string serverId)
    {
        var path = ServerPathTemplate
            .Replace("{serverId}", serverId, StringComparison.Ordinal)
            .Trim();

        if (!path.StartsWith('/'))
            path = $"/{path}";

        return $"{WebAppUrl.TrimEnd('/')}{path}";
    }
}
