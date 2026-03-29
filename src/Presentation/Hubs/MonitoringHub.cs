using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

[Authorize]
public class MonitoringHub : Hub
{
    public async Task SubscribeToServer(string serverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, MonitoringHubGroups.Server(serverId));
    }

    public async Task UnsubscribeFromServer(string serverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, MonitoringHubGroups.Server(serverId));
    }
}
