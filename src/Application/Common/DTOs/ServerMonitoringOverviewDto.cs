using Domain.Entities;
using Domain.Enums;

namespace Application.Common.DTOs;

public sealed class ServerMonitoringOverviewDto
{
    public required TargetInfoDto Target { get; init; }
    public required UptimeStatsDto Summary { get; init; }
    public required ChartDto Chart { get; init; }
}

public sealed class TargetInfoDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Host { get; init; }
    public string? Query { get; init; }
    public required int Port { get; init; }
    public required bool IsActive { get; init; }
    public required Protocol Protocol { get; init; }
    public required ServerStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }

    public static TargetInfoDto FromEntity(Server server)
    {
        return new TargetInfoDto
        {
            Id = server.Id ?? string.Empty,
            Name = server.Name ?? string.Empty,
            Host = server.Host ?? string.Empty,
            Query = server.Query,
            Port = server.Port ?? 0,
            IsActive = server.IsActive,
            Protocol = server.Protocol,
            Status = server.Status,
            CreatedAt = server.CreatedAt,
            UpdatedAt = server.ModifiedAt,
        };
    }
}
