using Domain.Enums;

namespace Presentation.Hubs;

public class ServerStatusChangedHubMessage
{
    public string ServerId { get; init; } = null!;
    public ServerStatus Status { get; init; }
}
