using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IServerStateRepository
{
    Task<ServerState?> GetByServerIdAsync(string serverId, CancellationToken cancellationToken);
}
