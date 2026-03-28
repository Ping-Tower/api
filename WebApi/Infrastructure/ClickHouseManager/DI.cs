using Application.Common.Interfaces;
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

        services.AddScoped<IClickHouseService, ClickHouseService>();

        return services;
    }
}
