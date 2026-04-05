using System.Text;
using System.Text.Json;
using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Infrastructure.RabbitMqManager;

public class ServerStatusRabbitMqWorker : BackgroundService
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    private readonly ILogger<ServerStatusRabbitMqWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private IConnection? _connection;
    private IChannel? _channel;

    public ServerStatusRabbitMqWorker(
        ILogger<ServerStatusRabbitMqWorker> logger,
        IServiceScopeFactory scopeFactory,
        RabbitMqSettings rabbitMqSettings)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _rabbitMqSettings = rabbitMqSettings;
    }

    private async Task ConnectToRabbitMq(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName!,
            Port = _rabbitMqSettings.Port,
            UserName = _rabbitMqSettings.UserName!,
            Password = _rabbitMqSettings.Password!
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_channel is null || _channel.IsClosed)
                    await ConnectToRabbitMq(stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel!);
                consumer.ReceivedAsync += async (_, ea) =>
                {
                    try
                    {
                        var rawMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var message = JsonSerializer.Deserialize<ServerStatusChangedMessage>(rawMessage, JsonSerializerOptions);
                        if (message is null || string.IsNullOrWhiteSpace(message.ServerId))
                        {
                            _logger.LogWarning("Invalid server status message: {Message}", rawMessage);
                            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                            return;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var serverRepository = scope.ServiceProvider.GetRequiredService<IServerRepository>();
                        var serverStatusChangeProcessor = scope.ServiceProvider.GetRequiredService<IServerStatusChangeProcessor>();
                        var statusChanged = await serverRepository.UpdateStatusAsync(message.ServerId, message.Status, stoppingToken);
                        if (statusChanged)
                            await serverStatusChangeProcessor.ProcessAsync(message.ServerId, message.Status, stoppingToken);

                        _logger.LogInformation(
                            "Server status updated. ServerId: {ServerId}, Status: {Status}",
                            message.ServerId,
                            message.Status);

                        await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process server status message.");
                        await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                await _channel!.BasicConsumeAsync(
                    queue: _rabbitMqSettings.ServerStatusChangedQueue,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                while (!_channel!.IsClosed && !stoppingToken.IsCancellationRequested)
                    await Task.Delay(1000, stoppingToken);
            }
            catch (OperationInterruptedException ex) when (ex.ShutdownReason?.Initiator == ShutdownInitiator.Peer)
            {
                _logger.LogWarning("RabbitMQ connection lost. Reconnecting in 5 seconds. Reason: {Reason}", ex.ShutdownReason?.ToString());
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in server status RabbitMQ worker. Reconnecting in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
            finally
            {
                if (_channel is not null && !_channel.IsClosed)
                {
                    await _channel.CloseAsync(stoppingToken);
                    _channel.Dispose();
                    _channel = null;
                }

                if (_connection is not null && _connection.IsOpen)
                {
                    await _connection.CloseAsync(stoppingToken);
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
            _channel.Dispose();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
