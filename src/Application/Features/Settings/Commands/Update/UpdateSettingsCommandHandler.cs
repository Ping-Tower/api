using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Settings.Commands.Update;

public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, Unit>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IServerEventPublisher _serverEventPublisher;
    private readonly IUserContext _userContext;

    public UpdateSettingsCommandHandler(
        IServerRepository serverRepository,
        ISettingsRepository settingsRepository,
        IServerEventPublisher serverEventPublisher,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _settingsRepository = settingsRepository;
        _serverEventPublisher = serverEventPublisher;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        var ping = await _settingsRepository.GetPingSettingsByServerIdAsync(request.ServerId, cancellationToken)
            ?? new Domain.Entities.PingSettings
            {
                Id = Guid.NewGuid().ToString(),
                ServerId = request.ServerId,
                IntervalSec = 60,
                LatencyThresholdMs = 400,
                Retries = 0,
                FailureThreshold = 1
            };

        ping.IntervalSec = request.IntervalSec ?? ping.IntervalSec;
        ping.LatencyThresholdMs = request.LatencyThresholdMs ?? ping.LatencyThresholdMs;
        ping.Retries = request.Retries ?? ping.Retries;
        ping.FailureThreshold = request.FailureThreshold ?? ping.FailureThreshold;

        await _settingsRepository.UpsertPingSettingsAsync(ping, cancellationToken);
        await _serverEventPublisher.PublishServerEditedAsync(server, ping, cancellationToken);

        return Unit.Value;
    }
}
