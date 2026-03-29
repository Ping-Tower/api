using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Infrastructure.RabbitMqManager;

public class RabbitMqHostedService : IRabbitMqProvider, IHostedService, IAsyncDisposable
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private IConnection? _connection;
    public IConnection Connection => _connection ?? throw new Exception("RabbitMq connection not initialized");

    public RabbitMqHostedService(RabbitMqSettings rabbitMqSettings)
    {
        _rabbitMqSettings = rabbitMqSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqSettings.HostName!,
            Port = _rabbitMqSettings.Port,
            UserName = _rabbitMqSettings.UserName!,
            Password = _rabbitMqSettings.Password!
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if(_connection is not null && _connection.IsOpen)
            await _connection.CloseAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if(_connection is not null)
            await _connection.DisposeAsync();
    }
}
