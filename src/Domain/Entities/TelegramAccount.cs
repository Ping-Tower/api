using Domain.Common.Interfaces;

namespace Domain.Entities;

public class TelegramAccount : BaseEntity
{
    public long TelegramUserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string? Username { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTimeOffset AuthDateUtc { get; set; }
    public string? UserId { get; set; }
}
