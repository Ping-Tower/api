using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Presentation.Hubs;

[Authorize]
public class MonitoringHub : Hub
{
    public async Task SubscribeToServer(int serverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"server-{serverId}");
    }

    public async Task UnsubscribeFromServer(int serverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"server-{serverId}");
    }
}
