using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Infrastructure.EmailManager;

namespace Infrastructure.RabbitMqManager;

public static class DI
{
    public static IServiceCollection ApplyRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var sectionName = "RabbitMqSettings";
        services.Configure<RabbitMqSettings>(configuration.GetSection(sectionName));
        var rabbitMqSettings = configuration.GetSection(sectionName).Get<RabbitMqSettings>()
            ?? new RabbitMqSettings();
        services.AddSingleton(rabbitMqSettings);

        services.AddSingleton<RabbitMqHostedService>();
        services.AddSingleton<IRabbitMqProvider>(sp => sp.GetRequiredService<RabbitMqHostedService>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqHostedService>());
        services.AddHostedService<ServerStatusRabbitMqWorker>();

        services.AddSingleton<IEmailService, EmailPublisher>();
        services.AddSingleton<IServerEventPublisher, ServerEventPublisher>();
        services.AddHealthChecks()
            .AddAsyncCheck(
                name: "RabbitMQ Broker",
                check: async cancellationToken =>
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = rabbitMqSettings.HostName ?? throw new InvalidOperationException("RabbitMQ host is missing."),
                        Port = rabbitMqSettings.Port,
                        UserName = rabbitMqSettings.UserName ?? throw new InvalidOperationException("RabbitMQ username is missing."),
                        Password = rabbitMqSettings.Password ?? throw new InvalidOperationException("RabbitMQ password is missing.")
                    };

                    await using var connection = await factory.CreateConnectionAsync(cancellationToken);
                    await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                    return connection.IsOpen && !channel.IsClosed
                        ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
                        : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ connection or channel is closed.");
                },
                tags: ["broker", "ready"],
                timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}
