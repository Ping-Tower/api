using Application.Common.Interfaces;
using Infrastructure.DataManager;
using Infrastructure.IdentityManager.Tokens;
using Infrastructure.Identity.AspNetCoreIdentity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.IdentityManager.AspNetCoreIdentity;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ApplyDataManager(configuration);

        services.ApplyIdentity(configuration);

        services.ApplyTokenManager(configuration);

        services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }
}