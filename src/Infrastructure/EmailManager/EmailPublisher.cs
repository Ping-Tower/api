using System.Text;
using System.Text.Json;
using Application.Common.Interfaces;
using Infrastructure.RabbitMqManager;
using RabbitMQ.Client;

namespace Infrastructure.EmailManager;

public class EmailPublisher : IEmailService
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IRabbitMqProvider _rabbitMqProvider;

    public EmailPublisher(RabbitMqSettings rabbitMqSettings, IRabbitMqProvider rabbitMqProvider)
    {
        _rabbitMqProvider = rabbitMqProvider;
        _rabbitMqSettings = rabbitMqSettings;
    }
    public async Task SendMessage(string email, string subject, string htmlbody,  CancellationToken cancellationToken)
    {
        await using var channel = await _rabbitMqProvider.Connection.CreateChannelAsync(
            new CreateChannelOptions(true, true), cancellationToken);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new MessageDto
        {
            Email = email,
            Subject = subject,
            HtmlBody = htmlbody
        }));

        var props = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                {"x-retry-count", 0}
            }
        };

        await channel.BasicPublishAsync(
        exchange: "",
        routingKey: _rabbitMqSettings.MainQueue,
        mandatory: true,
        basicProperties: props,
        body: body);
    }
}