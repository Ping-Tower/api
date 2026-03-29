using Application.Common.DTOs;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Settings.Queries.Get;

public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, SettingsDto>
{
    private readonly IServerRepository _serverRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IUserContext _userContext;

    public GetSettingsQueryHandler(
        IServerRepository serverRepository,
        ISettingsRepository settingsRepository,
        IUserContext userContext)
    {
        _serverRepository = serverRepository;
        _settingsRepository = settingsRepository;
        _userContext = userContext;
    }

    public async Task<SettingsDto> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var server = await _serverRepository.GetByIdAsync(request.ServerId, cancellationToken)
            ?? throw new NotFoundException($"Server {request.ServerId} not found.");

        if (server.UserId != _userContext.UserId)
            throw new UnauthorizedException("Access denied.");

        var ping = await _settingsRepository.GetPingSettingsByServerIdAsync(request.ServerId, cancellationToken);
        var notification = await _settingsRepository.GetNotificationSettingsByUserIdAsync(_userContext.UserId!, cancellationToken);

        return new SettingsDto
        {
            PingSettings = ping == null ? null : new PingSettingsDto
            {
                Id = ping.Id,
                IntervalSec = ping.IntervalSec,
                LatencyThresholdMs = ping.LatencyThresholdMs,
                Retries = ping.Retries,
                FailureThreshold = ping.FailureThreshold
            },
            NotificationSettings = notification == null ? null : new NotificationSettingsDto
            {
                Id = notification.Id,
                OnDown = notification.OnDown,
                OnUp = notification.OnUp,
                OnLatency = notification.OnLatency,
                CooldownSec = notification.CooldownSec
            }
        };
    }
}
