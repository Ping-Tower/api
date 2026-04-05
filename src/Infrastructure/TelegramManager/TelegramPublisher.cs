using System.Text;
using System.Text.Json;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Infrastructure.RabbitMqManager;
using RabbitMQ.Client;

namespace Infrastructure.TelegramManager;

public class TelegramPublisher : ITelegramNotificationService
{
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IRabbitMqProvider _rabbitMqProvider;

    public TelegramPublisher(RabbitMqSettings rabbitMqSettings, IRabbitMqProvider rabbitMqProvider)
    {
        _rabbitMqProvider = rabbitMqProvider;
        _rabbitMqSettings = rabbitMqSettings;
    }

    public async Task SendMessageAsync(
        long chatId,
        string text,
        IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons,
        CancellationToken cancellationToken)
    {
        await using var channel = await _rabbitMqProvider.Connection.CreateChannelAsync(
            new CreateChannelOptions(true, true),
            cancellationToken);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(
            new TelegramMessageDto
            {
                ChatId = chatId,
                Text = text,
                InlineButtons = MapInlineButtons(inlineButtons)
            },
            RabbitMqJsonSerializer.Options));

        var props = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                { "x-retry-count", 0 }
            }
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: _rabbitMqSettings.TelegramQueue,
            mandatory: true,
            basicProperties: props,
            body: body);
    }

    private static List<List<TelegramInlineButtonMessageDto>> MapInlineButtons(
        IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons)
    {
        if (inlineButtons is null || inlineButtons.Count == 0)
            return [];

        return inlineButtons
            .Select(row => row
                .Select(button => new TelegramInlineButtonMessageDto
                {
                    Text = button.Text,
                    Url = button.Url,
                    CallbackData = button.CallbackData
                })
                .ToList())
            .ToList();
    }
}
