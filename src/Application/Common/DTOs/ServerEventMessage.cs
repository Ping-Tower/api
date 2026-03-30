using Domain.Enums;

namespace Application.Common.DTOs;

public class ServerEventMessage
{
    public ServerEventServerDto Server { get; set; } = null!;
    public ServerEventPingSettingsDto PingSettings { get; set; } = null!;
}

public class ServerEventServerDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Host { get; set; }
    public string? Query { get; set; }
    public string? UserId { get; set; }
    public int? Port { get; set; }
    public bool IsActive { get; set; }
    public Protocol Protocol { get; set; }
    public ServerStatus Status { get; set; }
    public bool IsDeleted { get; set; }
}

public class ServerEventPingSettingsDto
{
    public string? Id { get; set; }
    public string? ServerId { get; set; }
    public int? IntervalSec { get; set; }
    public int? LatencyThresholdMs { get; set; }
    public int? Retries { get; set; }
    public int? FailureThreshold { get; set; }
    public bool IsDeleted { get; set; }
}
