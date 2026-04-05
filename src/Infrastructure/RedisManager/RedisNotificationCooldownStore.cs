using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.RedisManager;

public class RedisNotificationCooldownStore : INotificationCooldownStore
{
    private readonly IDatabase _database;
    private readonly string _keyPrefix;

    public RedisNotificationCooldownStore(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisSettings> settings)
    {
        _database = connectionMultiplexer.GetDatabase();
        _keyPrefix = settings.Value.KeyPrefix;
    }

    public async Task<bool> IsInCooldownAsync(string serverId, ServerStatus status, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _database.KeyExistsAsync(CooldownKey(serverId, status));
    }

    public async Task SetCooldownAsync(string serverId, ServerStatus status, TimeSpan ttl, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (ttl <= TimeSpan.Zero)
            return;

        await _database.StringSetAsync(
            CooldownKey(serverId, status),
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ttl);
    }

    private string CooldownKey(string serverId, ServerStatus status) => $"{_keyPrefix}:cooldown:server:{serverId}:status:{status}";
}
