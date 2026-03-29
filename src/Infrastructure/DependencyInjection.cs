using Application.Common.Services.IdentityManager;
using Infrastructure.DataManager;
using Infrastructure.IdentityManager.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Infrastructure.RabbitMqManager;
using Infrastructure.ClickHouseManager;
using Infrastructure.TelegramAuth;

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

        var telegramAuthSettings = configuration.GetSection("TelegramAuthSettings").Get<TelegramAuthSettings>()
            ?? new TelegramAuthSettings();
        services.AddSingleton(telegramAuthSettings);
        services.AddSingleton<Application.Common.Interfaces.ITelegramLoginValidator, TelegramLoginValidator>();

        return services;
    }
}
