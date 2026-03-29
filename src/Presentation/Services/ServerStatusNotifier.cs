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

    public Task NotifyStatusChangedAsync(string serverId, ServerStatus status, CancellationToken cancellationToken)
    {
        return _hubContext.Clients
            .Group(MonitoringHubGroups.Server(serverId))
            .SendAsync(
                MonitoringHubEvents.ServerStatusChanged,
                new ServerStatusChangedHubMessage
                {
                    ServerId = serverId,
                    Status = status
                },
                cancellationToken);
    }
}
