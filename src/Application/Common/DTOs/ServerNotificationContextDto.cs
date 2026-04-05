namespace Application.Common.DTOs;

public sealed class ServerNotificationContextDto
{
    public string ServerId { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public string? ServerName { get; init; }
    public string? Host { get; init; }
    public string? Email { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool OnDown { get; init; }
    public bool OnUp { get; init; }
    public int CooldownSec { get; init; }
    public IReadOnlyList<long> TelegramUserIds { get; init; } = [];
}
