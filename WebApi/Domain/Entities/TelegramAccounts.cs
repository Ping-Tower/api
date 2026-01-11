using Domain.Common.Interfaces;

namespace Domain.Entities;

public class TelegramAccount : BaseEntity
{
    public string? ChatId { get; set; }
    public string? UserId { get; set; }
}