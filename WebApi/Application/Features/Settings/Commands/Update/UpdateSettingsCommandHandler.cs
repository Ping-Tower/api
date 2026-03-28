using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Settings.Commands.Update;

public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, Unit>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IUserContext _userContext;

    public UpdateSettingsCommandHandler(
        IServerRepository serverRepository,
        ISettingsRepository settingsRepository,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _settingsRepository = settingsRepository;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizationException("Access denied.");

        var (ping, notification) = await _settingsRepository.GetByServerIdAsync(request.ServerId, cancellationToken);

        if (ping != null)
        {
            ping.IntervalSec = request.IntervalSec ?? ping.IntervalSec;
            ping.Retries = request.Retries ?? ping.Retries;
            ping.FailtureThreshold = request.FailureThreshold ?? ping.FailtureThreshold;
            await _settingsRepository.UpdatePingSettingsAsync(ping, cancellationToken);
        }

        if (notification != null)
        {
            notification.OnDown = request.OnDown ?? notification.OnDown;
            notification.OnUp = request.OnUp ?? notification.OnUp;
            notification.OnLatency = request.OnLatency ?? notification.OnLatency;
            notification.LatencyTresholdMs = request.LatencyThresholdMs ?? notification.LatencyTresholdMs;
            notification.CooldownSec = request.CooldownSec ?? notification.CooldownSec;
            await _settingsRepository.UpdateNotificationSettingsAsync(notification, cancellationToken);
        }

        return Unit.Value;
    }
}
