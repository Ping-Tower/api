using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IServerRepository
{
    Task<List<Server>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task<Server?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Server server, CancellationToken cancellationToken);
    Task UpdateAsync(Server server, CancellationToken cancellationToken);
    Task DeleteAsync(Server server, CancellationToken cancellationToken);
}
