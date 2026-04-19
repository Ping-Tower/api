namespace Application.Common.DTOs;

public class UptimeStatsDto
{
    public long TotalChecks { get; set; }
    public long SuccessfulChecks { get; set; }
    public double UptimePct { get; set; }
    public double? AvgLatencyMs { get; set; }
    public double? P50LatencyMs { get; set; }
    public double? P90LatencyMs { get; set; }
    public double? P99LatencyMs { get; set; }
    public double? AvgDnsLookupMs { get; set; }
    public DateTime? EarliestCertExpiresAt { get; set; }
    public DateTime? LatestCertExpiresAt { get; set; }
    public List<TlsVersionStatDto> TlsVersions { get; set; } = [];
    public double? AvgPacketLossPercent { get; set; }
    public double? AvgRttMinMs { get; set; }
    public double? AvgRttMaxMs { get; set; }
    public double? AvgTtl { get; set; }
    public double? AvgSentBytes { get; set; }
    public double? AvgReceivedBytes { get; set; }
    public long? TotalSentBytes { get; set; }
    public long? TotalReceivedBytes { get; set; }
}

public sealed class TlsVersionStatDto
{
    public string Version { get; set; } = string.Empty;
    public long Count { get; set; }
    public double SharePct { get; set; }
}
