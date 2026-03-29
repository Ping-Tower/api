using System.Text;
using System.Text.Json;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMqManager;

public class ServerEventPublisher : IServerEventPublisher
{
    private readonly IRabbitMqProvider _rabbitMqProvider;
    private readonly RabbitMqSettings _rabbitMqSettings;

    public ServerEventPublisher(IRabbitMqProvider rabbitMqProvider, RabbitMqSettings rabbitMqSettings)
    {
        _rabbitMqProvider = rabbitMqProvider;
        _rabbitMqSettings = rabbitMqSettings;
    }

    public Task PublishServerAddedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
        PublishAsync("add", _rabbitMqSettings.ServerAddedRoutingKey, server, pingSettings, cancellationToken);

    public Task PublishServerEditedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
        PublishAsync("edit", _rabbitMqSettings.ServerEditedRoutingKey, server, pingSettings, cancellationToken);

    public Task PublishServerDeletedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
        PublishAsync("delete", _rabbitMqSettings.ServerDeletedRoutingKey, server, pingSettings, cancellationToken);

    private async Task PublishAsync(
        string action,
        string routingKey,
        Server server,
        PingSettings pingSettings,
        CancellationToken cancellationToken)
    {
        await using var channel = await _rabbitMqProvider.Connection.CreateChannelAsync(
            new CreateChannelOptions(true, true), cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ServerEventMessage
        {
            Action = action,
            Server = new ServerEventServerDto
            {
                Id = server.Id,
                Name = server.Name,
                Host = server.Host,
                UserId = server.UserId,
                Port = server.Port,
                IsActive = server.IsActive,
                Protocol = server.Protocol,
                Status = server.Status,
                IsDeleted = server.IsDeleted
            },
            PingSettings = new ServerEventPingSettingsDto
            {
                Id = pingSettings.Id,
                ServerId = pingSettings.ServerId,
                IntervalSec = pingSettings.IntervalSec,
                LatencyThresholdMs = pingSettings.LatencyThresholdMs,
                Retries = pingSettings.Retries,
                FailureThreshold = pingSettings.FailureThreshold,
                IsDeleted = pingSettings.IsDeleted
            }
        }));

        var props = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: _rabbitMqSettings.ServerEventsExchange,
            routingKey: routingKey,
            mandatory: true,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);
    }
}
