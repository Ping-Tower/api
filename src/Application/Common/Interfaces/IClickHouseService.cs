using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface IClickHouseService
{
    Task<List<PingRecord>> GetPingsAsync(string serverId, DateTime? from, DateTime? to, int limit, CancellationToken cancellationToken);
    Task<UptimeStatsDto> GetUptimeStatsAsync(string serverId, DateTime from, DateTime to, CancellationToken cancellationToken);
    Task<ChartDto> GetChartAsync(string serverId, DateTime from, DateTime to, int bucketSec, CancellationToken cancellationToken);
}
