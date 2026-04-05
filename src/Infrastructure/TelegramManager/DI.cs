using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.TelegramManager;

public static class DI
{
    public static IServiceCollection ApplyTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        var telegramAuthSettings = configuration.GetSection("TelegramAuthSettings").Get<TelegramAuthSettings>()
            ?? new TelegramAuthSettings();

        services.AddSingleton(telegramAuthSettings);
        services.AddSingleton<ITelegramLoginValidator, TelegramLoginValidator>();
        services.AddSingleton<ITelegramNotificationService, TelegramPublisher>();

        return services;
    }
}
