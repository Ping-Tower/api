using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;

namespace Presentation.Services;

public class ServerStatusNotifier : IServerStatusNotifier
{
    private readonly IHubContext<MonitoringHub> _hubContext;

    public ServerStatusNotifier(IHubContext<MonitoringHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyStatusChangedAsync(string userId, string serverId, ServerStatus status, CancellationToken cancellationToken)
    {
        var message = new ServerStatusChangedHubMessage
        {
            ServerId = serverId,
            Status = status
        };

        return _hubContext.Clients
            .User(userId)
            .SendAsync(
                MonitoringHubEvents.ServerStatusChanged,
                message,
                cancellationToken);
    }
}
