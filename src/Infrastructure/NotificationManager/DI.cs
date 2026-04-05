using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.NotificationManager;

public static class DI
{
    public static IServiceCollection ApplyNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        var appLinksSettings = configuration.GetSection("AppLinksSettings").Get<AppLinksSettings>()
            ?? new AppLinksSettings();

        services.AddSingleton(appLinksSettings);
        services.AddScoped<IServerStatusChangeProcessor, ServerStatusChangeProcessor>();

        return services;
    }
}
