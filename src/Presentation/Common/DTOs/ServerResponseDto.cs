using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;

namespace Presentation.Common.DTOs;

public sealed class ServerResponseDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Host { get; init; }
    public string? Query { get; init; }
    public required string UserId { get; init; }
    public required int Port { get; init; }
    public required bool IsActive { get; init; }
    public required Protocol Protocol { get; init; }
    public required ServerStatus Status { get; init; }

    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updatedAt")]
    public required DateTimeOffset UpdatedAt { get; init; }

    public static ServerResponseDto FromEntity(Server server)
    {
        return new ServerResponseDto
        {
            Id = server.Id ?? string.Empty,
            Name = server.Name ?? string.Empty,
            Host = server.Host ?? string.Empty,
            Query = server.Query,
            UserId = server.UserId ?? string.Empty,
            Port = server.Port ?? 0,
            IsActive = server.IsActive,
            Protocol = server.Protocol,
            Status = server.Status,
            CreatedAt = server.CreatedAt,
            UpdatedAt = server.ModifiedAt
        };
    }
}
