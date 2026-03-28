using Application.Common.DTOs;
using Application.Common.Interfaces;
using ClickHouse.Client.ADO;
using Dapper;
using Microsoft.Extensions.Options;

namespace Infrastructure.ClickHouseManager;

public class ClickHouseService(IOptions<ClickHouseSettings> options) : IClickHouseService
{
    private readonly ClickHouseSettings _settings = options.Value;

    public async Task<List<PingRecord>> GetPingsAsync(long serverId, DateTime? from, DateTime? to, int limit, CancellationToken cancellationToken)
    {
        await using var connection = new ClickHouseConnection(_settings.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var sql = """
            SELECT
                id AS Id,
                server_id AS ServerId,
                protocol AS Protocol,
                timestamp AS Timestamp,
                is_up AS IsUp,
                latency_ms AS LatencyMs,
                error_message AS ErrorMessage,
                status_code AS StatusCode,
                cert_expires_at AS CertExpiresAt,
                tls_version AS TlsVersion,
                dns_lookup_ms AS DnsLookupMs,
                sent_bytes AS SentBytes,
                received_bytes AS ReceivedBytes,
                expected_response_match AS ExpectedResponseMatch,
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
}
