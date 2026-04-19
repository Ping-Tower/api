using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.NotificationSettings.Queries.Get;

public class GetNotificationSettingsQueryHandler : IRequestHandler<GetNotificationSettingsQuery, NotificationSettingsDto>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IUserContext _userContext;

    public GetNotificationSettingsQueryHandler(
        ISettingsRepository settingsRepository,
        IUserContext userContext)
    {
        _settingsRepository = settingsRepository;
        _userContext = userContext;
    }

    public async Task<NotificationSettingsDto> Handle(GetNotificationSettingsQuery request, CancellationToken cancellationToken)
    {
        var notification = await _settingsRepository.GetNotificationSettingsByUserIdAsync(_userContext.UserId!, cancellationToken);

        return new NotificationSettingsDto
        {
            Id = notification?.Id,
            OnDown = notification?.OnDown ?? true,
            OnUp = notification?.OnUp ?? true,
            OnLatency = notification?.OnLatency ?? true,
            CooldownSec = notification?.CooldownSec ?? 600
        };
    }
}
