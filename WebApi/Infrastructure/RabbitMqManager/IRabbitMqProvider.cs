using RabbitMQ.Client;

namespace Infrastructure.RabbitMqManager;

public interface IRabbitMqProvider
{
    IConnection Connection { get; }
}