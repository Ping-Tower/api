using Application.Common.CQRS;
using Application.Common.DTOs;
using MediatR;

namespace Application.Features.NotificationSettings.Commands.Patch;

public record PatchNotificationSettingsCommand(
    bool? OnDown,
    bool? OnUp,
    bool? OnLatency,
    int? CooldownSec) : IRequest<NotificationSettingsDto>, ICommand;
