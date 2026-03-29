using Application.Common.CQRS;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.Features.TelegramAccounts.Commands.Create;

public record CreateTelegramAccountCommand(
    [property: JsonPropertyName("id")] long TelegramUserId,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("photo_url")] string? PhotoUrl,
    [property: JsonPropertyName("auth_date")] long AuthDate,
    [property: JsonPropertyName("hash")]
    string Hash) : IRequest<string>, ICommand;
