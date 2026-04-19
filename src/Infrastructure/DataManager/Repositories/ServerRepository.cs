using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
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

    public async Task<List<Server>> GetAllByUserIdAsync(string userId, string? search, CancellationToken cancellationToken)
    {
        return await _dbContext.Servers
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .Where(s => search == null || s.Name.Contains(search))
            .ToListAsync(cancellationToken);
    }

    public async Task<Server?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _dbContext.Servers
            .Include(s => s.PingSettingsRef)
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

    public async Task<bool> UpdateStatusAsync(string id, ServerStatus status, CancellationToken cancellationToken)
    {
        var server = await _dbContext.Servers
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

        if (server is null || server.Status == status)
            return false;

        server.Status = status;
        _dbContext.Servers.Update(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task DeleteAsync(Server server, CancellationToken cancellationToken)
    {
        server.IsDeleted = true;
        _dbContext.Servers.Update(server);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
