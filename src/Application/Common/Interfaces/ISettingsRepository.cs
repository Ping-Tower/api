using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ISettingsRepository
{
    Task<PingSettings?> GetPingSettingsByServerIdAsync(string serverId, CancellationToken cancellationToken);
    Task<NotificationSettings?> GetNotificationSettingsByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task UpsertPingSettingsAsync(PingSettings settings, CancellationToken cancellationToken);
    Task UpsertNotificationSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken);
}
