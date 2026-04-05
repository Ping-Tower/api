using Application.Common.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.RedisManager;

public static class DI
{
    public static IServiceCollection ApplyRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
        var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>()
            ?? new RedisSettings();
        var redisConnection = ConnectionMultiplexer.Connect(redisSettings.Configuration);

        services.AddSingleton(redisSettings);
        services.AddSingleton(redisConnection);
        services.AddSingleton<IConnectionMultiplexer>(redisConnection);
        services.AddDataProtection()
            .SetApplicationName("PingTower")
            .PersistKeysToStackExchangeRedis(redisConnection, "DataProtection-Keys");
        services.AddSingleton<INotificationCooldownStore, RedisNotificationCooldownStore>();

        services.AddHealthChecks()
            .AddRedis(
                redisConnectionString: redisSettings.Configuration,
                name: "Redis Cache",
                tags: ["cache", "ready"],
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
