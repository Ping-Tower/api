using Application.Common.DTOs;
using Application.Common.Interfaces;
using ClickHouse.Client.ADO;
using Dapper;
using Microsoft.Extensions.Options;

namespace Infrastructure.ClickHouseManager;

public class ClickHouseService(IOptions<ClickHouseSettings> options) : IClickHouseService
{
    private readonly ClickHouseSettings _settings = options.Value;

    public async Task<List<PingRecord>> GetPingsAsync(string serverId, DateTime? from, DateTime? to, int limit, CancellationToken cancellationToken)
    {
        await using var connection = new ClickHouseConnection(_settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = """
            SELECT
                id AS Id,
                server_id AS ServerId,
                protocol AS Protocol,
                timestamp AS Timestamp,
                is_success AS IsSuccess,
                latency_ms AS LatencyMs,
                error_message AS ErrorMessage,
                status_code AS StatusCode,
                cert_expires_at AS CertExpiresAt,
                tls_version AS TlsVersion,
                dns_lookup_ms AS DnsLookupMs,
                sent_bytes AS SentBytes,
                received_bytes AS ReceivedBytes,
                packet_loss_percent AS PacketLossPercent,
                rtt_min_ms AS RttMinMs,
                rtt_max_ms AS RttMaxMs,
                ttl AS Ttl
            FROM server_pings
            WHERE server_id = @ServerId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("ServerId", serverId);

        if (from.HasValue)
        {
            sql += " AND timestamp >= @From";
            parameters.Add("From", from.Value);
        }

        if (to.HasValue)
        {
            sql += " AND timestamp <= @To";
            parameters.Add("To", to.Value);
        }

        sql += " ORDER BY timestamp DESC LIMIT @Limit";
        parameters.Add("Limit", limit);

        return [.. await connection.QueryAsync<PingRecord>(sql, parameters)];
    }

    public async Task<UptimeStatsDto> GetUptimeStatsAsync(string serverId, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        await using var connection = new ClickHouseConnection(_settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT
                toInt64(count())                                                AS TotalChecks,
                toInt64(countIf(is_success))                                   AS SuccessfulChecks,
                if(count() > 0, countIf(is_success) * 100.0 / count(), 0.0)   AS UptimePct,
                avgIf(latency_ms, is_success)                                  AS AvgLatencyMs,
                quantileIf(0.5)(latency_ms, is_success AND latency_ms IS NOT NULL)  AS P50LatencyMs,
                quantileIf(0.9)(latency_ms, is_success AND latency_ms IS NOT NULL)  AS P90LatencyMs,
                quantileIf(0.99)(latency_ms, is_success AND latency_ms IS NOT NULL) AS P99LatencyMs,
                avgIf(dns_lookup_ms, dns_lookup_ms IS NOT NULL)                AS AvgDnsLookupMs,
                minIf(cert_expires_at, cert_expires_at IS NOT NULL)            AS EarliestCertExpiresAt,
                maxIf(cert_expires_at, cert_expires_at IS NOT NULL)            AS LatestCertExpiresAt,
                avgIf(packet_loss_percent, packet_loss_percent IS NOT NULL)    AS AvgPacketLossPercent,
                avgIf(rtt_min_ms, rtt_min_ms IS NOT NULL)                      AS AvgRttMinMs,
                avgIf(rtt_max_ms, rtt_max_ms IS NOT NULL)                      AS AvgRttMaxMs,
                avgIf(toFloat64(ttl), ttl IS NOT NULL)                         AS AvgTtl,
                avgIf(toFloat64(sent_bytes), sent_bytes IS NOT NULL)           AS AvgSentBytes,
                avgIf(toFloat64(received_bytes), received_bytes IS NOT NULL)   AS AvgReceivedBytes,
                if(
                    countIf(sent_bytes IS NOT NULL) > 0,
                    toInt64(sumIf(sent_bytes, sent_bytes IS NOT NULL)),
                    CAST(NULL, 'Nullable(Int64)')
                )                                                              AS TotalSentBytes,
                if(
                    countIf(received_bytes IS NOT NULL) > 0,
                    toInt64(sumIf(received_bytes, received_bytes IS NOT NULL)),
                    CAST(NULL, 'Nullable(Int64)')
                )                                                              AS TotalReceivedBytes
            FROM server_pings
            WHERE server_id = @ServerId
              AND timestamp >= @From
              AND timestamp <= @To
            """;

        var parameters = new DynamicParameters();
        parameters.Add("ServerId", serverId);
        parameters.Add("From", from);
        parameters.Add("To", to);

        var stats = await connection.QuerySingleAsync<UptimeStatsDto>(sql, parameters);

        const string tlsSql = """
            SELECT
                tls_version AS Version,
                toInt64(count()) AS Count
            FROM server_pings
            WHERE server_id = @ServerId
              AND timestamp >= @From
              AND timestamp <= @To
              AND tls_version IS NOT NULL
            GROUP BY Version
            ORDER BY Count DESC, Version ASC
            LIMIT 8
            """;

        var tlsVersions = (await connection.QueryAsync<TlsVersionStatDto>(tlsSql, parameters)).ToList();
        var totalTlsCount = tlsVersions.Sum(version => version.Count);

        stats.TlsVersions = totalTlsCount == 0
            ? []
            : [.. tlsVersions.Select(version => new TlsVersionStatDto
            {
                Version = version.Version,
                Count = version.Count,
                SharePct = Math.Round(version.Count * 100.0 / totalTlsCount, 2, MidpointRounding.AwayFromZero),
            })];

        return stats;
    }

    public async Task<ChartDto> GetChartAsync(string serverId, DateTime from, DateTime to, int bucketSec, CancellationToken cancellationToken)
    {
        await using var connection = new ClickHouseConnection(_settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        const string bucketsSql = """
            SELECT
                toStartOfInterval(timestamp, toIntervalSecond(@BucketSec)) AS Bucket,
                toInt64(count())                                            AS TotalChecks,
                toInt64(countIf(is_success))                               AS SuccessChecks,
                avgIf(latency_ms, is_success AND latency_ms IS NOT NULL)   AS AvgLatencyMs,
                quantileIf(0.5)(latency_ms, is_success AND latency_ms IS NOT NULL)  AS P50LatencyMs,
                quantileIf(0.9)(latency_ms, is_success AND latency_ms IS NOT NULL)  AS P90LatencyMs,
                quantileIf(0.99)(latency_ms, is_success AND latency_ms IS NOT NULL) AS P99LatencyMs,
                avgIf(dns_lookup_ms, dns_lookup_ms IS NOT NULL)            AS AvgDnsLookupMs,
                avgIf(packet_loss_percent, packet_loss_percent IS NOT NULL) AS AvgPacketLossPercent,
                avgIf(rtt_min_ms, rtt_min_ms IS NOT NULL)                  AS AvgRttMinMs,
                avgIf(rtt_max_ms, rtt_max_ms IS NOT NULL)                  AS AvgRttMaxMs,
                avgIf(toFloat64(ttl), ttl IS NOT NULL)                     AS AvgTtl,
                avgIf(toFloat64(sent_bytes), sent_bytes IS NOT NULL)       AS AvgSentBytes,
                avgIf(toFloat64(received_bytes), received_bytes IS NOT NULL) AS AvgReceivedBytes
            FROM server_pings
            WHERE server_id = @ServerId
              AND timestamp >= @From
              AND timestamp <= @To
            GROUP BY Bucket
            ORDER BY Bucket ASC
            """;

        var bucketsParams = new DynamicParameters();
        bucketsParams.Add("ServerId", serverId);
        bucketsParams.Add("From", from);
        bucketsParams.Add("To", to);
        bucketsParams.Add("BucketSec", bucketSec);

        var buckets = await connection.QueryAsync<MetricsBucketDto>(bucketsSql, bucketsParams);

        const string statusCodesSql = """
            SELECT
                toString(status_code) AS Code,
                toInt64(count())      AS Count
            FROM server_pings
            WHERE server_id = @ServerId
              AND timestamp >= @From
              AND timestamp <= @To
              AND status_code IS NOT NULL
            GROUP BY Code
            ORDER BY Count DESC
            LIMIT 10
            """;

        var statusParams = new DynamicParameters();
        statusParams.Add("ServerId", serverId);
        statusParams.Add("From", from);
        statusParams.Add("To", to);

        var statusCodes = await connection.QueryAsync<StatusCodeCountDto>(statusCodesSql, statusParams);

        return new ChartDto
        {
            From = from,
            To = to,
            BucketSizeSec = bucketSec,
            Buckets = [.. buckets],
            StatusCodes = [.. statusCodes],
        };
    }
}
