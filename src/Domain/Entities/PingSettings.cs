using Domain.Common.Interfaces;

namespace Domain.Entities;

public class PingSettings : BaseEntity
{
    public string? ServerId { get; set; }
    public int? IntervalSec { get; set; }
    public int? LatencyThresholdMs { get; set; }
    public int? Retries { get; set; }
    public int? FailureThreshold { get; set; }
    public Server? ServerRef {get; set; }
}
