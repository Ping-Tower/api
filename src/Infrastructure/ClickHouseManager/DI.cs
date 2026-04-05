using Application.Common.Interfaces;
using ClickHouse.Client.ADO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ClickHouseManager;

public static class DI
{
    public static IServiceCollection ApplyClickHouse(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.Configure<ClickHouseSettings>(configuration.GetSection("ClickHouseSettings"));
        var clickHouseSettings = configuration.GetSection("ClickHouseSettings").Get<ClickHouseSettings>()
            ?? new ClickHouseSettings();

        services.AddScoped<IClickHouseService, ClickHouseService>();
        services.AddHealthChecks()
            .AddAsyncCheck(
                name: "ClickHouse Analytics",
                check: async cancellationToken =>
                {
                    await using var connection = new ClickHouseConnection(clickHouseSettings.ConnectionString);
                    await connection.OpenAsync(cancellationToken);

                    await using var command = connection.CreateCommand();
                    command.CommandText = "SELECT 1";
                    var result = await command.ExecuteScalarAsync(cancellationToken);

                    return result?.ToString() == "1"
                        ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
                        : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("ClickHouse health query returned an unexpected result.");
                },
                tags: ["analytics", "ready"],
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
