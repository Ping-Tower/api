using Domain.Common.Interfaces;
using Domain.Enums;

namespace Domain.Entities;

public class Server : BaseEntity
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? UserId { get; set; }
    public Protocol ProtocolRef { get; set; }
    public List<Request>? RequestsRefs { get; set; } 
}