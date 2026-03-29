using Application.Common.CQRS;
using MediatR;

namespace Application.Features.Settings.Commands.Update;

public record UpdateSettingsCommand(
    string ServerId,
    int? IntervalSec,
    int? LatencyThresholdMs,
    int? Retries,
    int? FailureThreshold) : IRequest<Unit>, ICommand;
