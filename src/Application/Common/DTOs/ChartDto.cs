namespace Application.Common.DTOs;

public class ChartDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int BucketSizeSec { get; set; }
    public List<MetricsBucketDto> Buckets { get; set; } = [];
    public List<StatusCodeCountDto> StatusCodes { get; set; } = [];
}

public class MetricsBucketDto
{
    public DateTime Bucket { get; set; }
    public long TotalChecks { get; set; }
    public long SuccessChecks { get; set; }
    public double? AvgLatencyMs { get; set; }
    public double? P50LatencyMs { get; set; }
    public double? P90LatencyMs { get; set; }
    public double? P99LatencyMs { get; set; }
    public double? AvgDnsLookupMs { get; set; }
    public double? AvgPacketLossPercent { get; set; }
    public double? AvgRttMinMs { get; set; }
    public double? AvgRttMaxMs { get; set; }
    public double? AvgTtl { get; set; }
    public double? AvgSentBytes { get; set; }
    public double? AvgReceivedBytes { get; set; }
}

public class StatusCodeCountDto
{
    public string Code { get; set; } = string.Empty;
    public long Count { get; set; }
}
