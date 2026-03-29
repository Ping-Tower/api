using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.RabbitMqManager;

internal static class RabbitMqJsonSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}
