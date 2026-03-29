using System.ComponentModel;
using System.Xml.Schema;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.Replication;
using RabbitMQ.Client;
using Infrastructure.EmailManager;

namespace Infrastructure.RabbitMqManager;

public static class DI
{
    public static IServiceCollection ApplyRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var sectionName = "RabbitMqSettings";
        services.Configure<RabbitMqSettings>(configuration.GetSection(sectionName));

        services.AddSingleton<RabbitMqHostedService>();
        services.AddSingleton<IRabbitMqProvider>(sp => sp.GetRequiredService<RabbitMqHostedService>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqHostedService>());
        services.AddHostedService<ServerStatusRabbitMqWorker>();

        services.AddSingleton<IEmailService, EmailPublisher>();
        services.AddSingleton<IServerEventPublisher, ServerEventPublisher>();

        return services;
    }
}
