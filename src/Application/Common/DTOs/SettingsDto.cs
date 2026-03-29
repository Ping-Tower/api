namespace Application.Common.DTOs;

public class PingSettingsDto
{
    public string? Id { get; set; }
    public int? IntervalSec { get; set; }
    public int? LatencyThresholdMs { get; set; }
    public int? Retries { get; set; }
    public int? FailureThreshold { get; set; }
}

public class NotificationSettingsDto
{
    public string? Id { get; set; }
    public bool? OnDown { get; set; }
    public bool? OnUp { get; set; }
    public bool? OnLatency { get; set; }
    public int? CooldownSec { get; set; }
}

public class SettingsDto
{
    public PingSettingsDto? PingSettings { get; set; }
    public NotificationSettingsDto? NotificationSettings { get; set; }
}
