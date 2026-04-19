using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IServerRepository
{
    Task<List<Server>> GetAllByUserIdAsync(string userId, string? search, CancellationToken cancellationToken);
    Task<Server?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Server server, CancellationToken cancellationToken);
    Task UpdateAsync(Server server, CancellationToken cancellationToken);
    Task<bool> UpdateStatusAsync(string id, Domain.Enums.ServerStatus status, CancellationToken cancellationToken);
    Task DeleteAsync(Server server, CancellationToken cancellationToken);
}
