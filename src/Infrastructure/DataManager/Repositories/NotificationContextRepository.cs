using Application.Common.DTOs;
using Application.Common.Interfaces;
using Infrastructure.DataManager.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Repositories;

public class NotificationContextRepository : INotificationContextRepository
{
    private readonly AppDbContext _dbContext;

    public NotificationContextRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServerNotificationContextDto?> GetServerNotificationContextAsync(string serverId, CancellationToken cancellationToken)
    {
        var rows = await (
            from server in _dbContext.Servers
            join user in _dbContext.Users on server.UserId equals user.Id
            join notificationSettings in _dbContext.NotificationSettings
                on user.Id equals notificationSettings.UserId into notificationSettingsGroup
            from notificationSettings in notificationSettingsGroup.DefaultIfEmpty()
            join telegramAccount in _dbContext.TelegramAccount.Where(account => !account.IsDeleted)
                on user.Id equals telegramAccount.UserId into telegramAccountGroup
            from telegramAccount in telegramAccountGroup.DefaultIfEmpty()
            where server.Id == serverId && !server.IsDeleted
            select new
            {
                server.Id,
                server.Name,
                server.Host,
                UserId = user.Id,
                user.Email,
                user.EmailConfirmed,
                OnDown = notificationSettings != null ? notificationSettings.OnDown : null,
                OnUp = notificationSettings != null ? notificationSettings.OnUp : null,
                CooldownSec = notificationSettings != null ? notificationSettings.CooldownSec : null,
                TelegramUserId = telegramAccount != null ? (long?)telegramAccount.TelegramUserId : null
            })
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
            return null;

        var context = rows[0];
        var telegramUserIds = rows
            .Where(row => row.TelegramUserId.HasValue)
            .Select(row => row.TelegramUserId!.Value)
            .Distinct()
            .ToList();

        return new ServerNotificationContextDto
        {
            ServerId = context.Id,
            UserId = context.UserId,
            ServerName = context.Name,
            Host = context.Host,
            Email = context.Email,
            EmailConfirmed = context.EmailConfirmed,
            OnDown = context.OnDown ?? true,
            OnUp = context.OnUp ?? true,
            CooldownSec = context.CooldownSec ?? 600,
            TelegramUserIds = telegramUserIds
        };
    }
}
