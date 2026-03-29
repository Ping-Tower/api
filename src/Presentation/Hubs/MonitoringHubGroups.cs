namespace Presentation.Hubs;

public static class MonitoringHubGroups
{
    public static string Server(string serverId) => $"server-{serverId}";
}
