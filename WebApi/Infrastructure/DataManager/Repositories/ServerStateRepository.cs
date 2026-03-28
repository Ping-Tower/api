using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Repositories;

public class ServerStateRepository : IServerStateRepository
{
    private readonly AppDbContext _dbContext;

    public ServerStateRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServerState?> GetByServerIdAsync(string serverId, CancellationToken cancellationToken)
    {
        return await _dbContext.ServerState
            .FirstOrDefaultAsync(s => s.ServerId == serverId, cancellationToken);
    }
}
