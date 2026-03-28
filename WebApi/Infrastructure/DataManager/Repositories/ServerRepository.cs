using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Repositories;

public class ServerRepository : IServerRepository
{
    private readonly AppDbContext _dbContext;

    public ServerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Server>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Servers
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Server?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _dbContext.Servers
            .Include(s => s.PingSettingsRef)
            .Include(s => s.NotificationSettingsRef)
            .Include(s => s.ServerStateRef)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task CreateAsync(Server server, CancellationToken cancellationToken)
    {
        await _dbContext.Servers.AddAsync(server, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Server server, CancellationToken cancellationToken)
    {
        _dbContext.Servers.Update(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Server server, CancellationToken cancellationToken)
    {
        server.IsDeleted = true;
        _dbContext.Servers.Update(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
