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

    public async Task<PingSettings?> GetPingSettingsByServerIdAsync(string serverId, CancellationToken cancellationToken)
    {
        return await _dbContext.PingSettings
            .FirstOrDefaultAsync(s => s.ServerId == serverId, cancellationToken);
    }

    public async Task<NotificationSettings?> GetNotificationSettingsByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _dbContext.NotificationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task UpsertPingSettingsAsync(PingSettings settings, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.PingSettings
            .AnyAsync(s => s.Id == settings.Id, cancellationToken);

        if (exists)
            _dbContext.PingSettings.Update(settings);
        else
            await _dbContext.PingSettings.AddAsync(settings, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertNotificationSettingsAsync(NotificationSettings settings, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.NotificationSettings
            .AnyAsync(s => s.Id == settings.Id, cancellationToken);

        if (exists)
            _dbContext.NotificationSettings.Update(settings);
        else
            await _dbContext.NotificationSettings.AddAsync(settings, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
