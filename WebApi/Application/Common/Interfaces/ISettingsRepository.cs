using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ISettingsRepository
{
    Task<(PingSettings? ping, NotificationSettings? notification)> GetByServerIdAsync(string serverId, CancellationToken cancellationToken);
    Task UpdatePingSettingsAsync(PingSettings settings, CancellationToken cancellationToken);
    Task UpdateNotificationSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken);
}
