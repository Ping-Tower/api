using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface INotificationContextRepository
{
    Task<ServerNotificationContextDto?> GetServerNotificationContextAsync(string serverId, CancellationToken cancellationToken);
}
