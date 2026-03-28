using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly AppDbContext _dbContext;

    public SettingsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(PingSettings? ping, NotificationSettings? notification)> GetByServerIdAsync(string serverId, CancellationToken cancellationToken)
    {
        var ping = await _dbContext.PingSettings
            .FirstOrDefaultAsync(s => s.ServerId == serverId, cancellationToken);

        var notification = await _dbContext.NotificationSettings
            .FirstOrDefaultAsync(s => s.ServerId == serverId, cancellationToken);

        return (ping, notification);
    }

    public async Task UpdatePingSettingsAsync(PingSettings settings, CancellationToken cancellationToken)
    {
        _dbContext.PingSettings.Update(settings);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateNotificationSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken)
    {
        _dbContext.NotificationSettings.Update(settings);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
