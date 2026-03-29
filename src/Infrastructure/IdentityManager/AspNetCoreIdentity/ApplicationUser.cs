using System.Dynamic;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public class ApplicationUser : IdentityUser
{
    public bool IsBlocked { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedById { get; set; }
    public List<Server>? ServerRefs { get; init; }
    public List<Token>? TokenRefs { get; init; }
    public TelegramAccount? TelegramAccountRef { get; init; }
    public NotificationSettings? NotificationSettingsRef { get; init; }
}
