using Domain.Enums;

namespace Application.Common.Interfaces;

public interface IServerStatusNotifier
{
    Task NotifyStatusChangedAsync(string serverId, ServerStatus status, CancellationToken cancellationToken);
}
