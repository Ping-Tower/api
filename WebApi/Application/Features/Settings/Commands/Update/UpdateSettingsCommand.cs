using Application.Common.CQRS;
using MediatR;

namespace Application.Features.Settings.Commands.Update;

public record UpdateSettingsCommand(
    string ServerId,
    int? IntervalSec,
    int? Retries,
    int? FailureThreshold,
    bool? OnDown,
    bool? OnUp,
    bool? OnLatency,
    int? LatencyThresholdMs,
    int? CooldownSec) : IRequest<Unit>, ICommand;
