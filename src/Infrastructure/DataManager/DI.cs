using Application.Common.Interfaces;
using Infrastructure.DataManager.Contexts;
using Infrastructure.DataManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataManager;

public static class DI
{
    public static IServiceCollection ApplyDataManager(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        if(string.IsNullOrEmpty(connectionString))
            throw new Exception("Postgres connectionstring is missing!");

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<ITelegramAccountRepository, TelegramAccountRepository>();
        services.AddScoped<INotificationContextRepository, NotificationContextRepository>();

        services.AddHealthChecks()
            .AddNpgSql(
                connectionString: connectionString,
                name: "PostgreSQL Database",
                healthQuery: "SELECT 1;",
                tags: ["db", "ready"],
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
