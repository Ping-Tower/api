using Domain.Common.Interfaces;
using Domain.Enums;

namespace Domain.Entities;

public class Server : BaseEntity
{
    public string? Name { get; set; }
    public string? Host { get; set; }
    public string? UserId { get; set; }
    public int? Port { get; set; }
    public bool IsActive { get; set; }
    public Protocol Protocol { get; set; }
    public ServerStatus Status { get; set; }
    public List<Request>? RequestsRefs { get; set; } 
    public PingSettings? PingSettingsRef { get; set; }
}
