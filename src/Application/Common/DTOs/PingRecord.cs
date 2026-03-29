namespace Application.Common.DTOs;

public class PingRecord
{
    public Guid Id { get; set; }
    public string ServerId { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public bool IsSuccess { get; set; }
    public double? LatencyMs { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }
    public DateTime? CertExpiresAt { get; set; }
    public string? TlsVersion { get; set; }
    public double? DnsLookupMs { get; set; }
    public long? SentBytes { get; set; }
    public long? ReceivedBytes { get; set; }
    public double? PacketLossPercent { get; set; }
    public double? RttMinMs { get; set; }
    public double? RttMaxMs { get; set; }
    public int? Ttl { get; set; }
}
