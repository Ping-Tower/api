using System.Diagnostics;
using Domain.Common.Interfaces;

namespace Domain.Entities;

public class ServerState : BaseEntity
{
    public string? ServerId { get; set; }
    public bool? IsUp { get; set; }
    public bool? FailCount { get; set; }
    public Server? ServerRef { get; set; }
}