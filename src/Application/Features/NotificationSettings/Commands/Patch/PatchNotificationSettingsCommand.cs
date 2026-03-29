using Application.Common.CQRS;
using MediatR;

namespace Application.Features.NotificationSettings.Commands.Patch;

public record PatchNotificationSettingsCommand(
    bool? OnDown,
    bool? OnUp,
    bool? OnLatency,
    int? CooldownSec) : IRequest<Unit>, ICommand;
