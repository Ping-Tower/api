using Application.Common.Services.IdentityManager;
using Infrastructure.DataManager;
using Infrastructure.IdentityManager.Tokens;
using Infrastructure.Identity.AspNetCoreIdentity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Infrastructure.RabbitMqManager;
using Infrastructure.ClickHouseManager;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ApplyDataManager(configuration);

        services.ApplyIdentity(configuration);

        services.ApplyTokenManager(configuration);

        services.AddTransient<IIdentityService, IdentityService>();

        services.ApplyRabbitMq(configuration);

        services.ApplyClickHouse(configuration);

        return services;
    }
}