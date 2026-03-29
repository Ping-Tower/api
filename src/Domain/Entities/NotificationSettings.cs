using Domain.Common.Interfaces;

namespace Domain.Entities;

public class NotificationSettings : BaseEntity
{
    public string? UserId { get; set; }
    public bool? OnDown { get; set; }
    public bool? OnUp { get; set; }
    public bool? OnLatency { get; set; }
    public int? CooldownSec { get; set; }
}
