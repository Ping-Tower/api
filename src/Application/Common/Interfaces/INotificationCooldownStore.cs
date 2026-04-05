using Domain.Enums;

namespace Application.Common.Interfaces;

public interface INotificationCooldownStore
{
    Task<bool> IsInCooldownAsync(string serverId, ServerStatus status, CancellationToken cancellationToken);
    Task SetCooldownAsync(string serverId, ServerStatus status, TimeSpan ttl, CancellationToken cancellationToken);
}
