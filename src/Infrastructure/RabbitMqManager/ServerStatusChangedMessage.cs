using System.Text.Json.Serialization;
using Domain.Enums;

namespace Infrastructure.RabbitMqManager;

public class ServerStatusChangedMessage
{
    [JsonPropertyName("server_id")]
    public string ServerId { get; init; } = null!;

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter<ServerStatus>))]
    public ServerStatus Status { get; init; }
}
