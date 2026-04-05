using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.NotificationSettings.Commands.Patch;

public class PatchNotificationSettingsCommandHandler : IRequestHandler<PatchNotificationSettingsCommand, NotificationSettingsDto>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IUserContext _userContext;

    public PatchNotificationSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        IUserContext userContext)
    {
        _settingsRepository = settingsRepository;
        _userContext = userContext;
    }

    public async Task<NotificationSettingsDto> Handle(PatchNotificationSettingsCommand request, CancellationToken cancellationToken)
    {
        var notification = await _settingsRepository.GetNotificationSettingsByUserIdAsync(_userContext.UserId!, cancellationToken)
            ?? new Domain.Entities.NotificationSettings
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userContext.UserId,
                OnDown = true,
                OnUp = true,
                OnLatency = true,
                CooldownSec = 600
            };

        notification.OnDown = request.OnDown ?? notification.OnDown;
        notification.OnUp = request.OnUp ?? notification.OnUp;
        notification.OnLatency = request.OnLatency ?? notification.OnLatency;
        notification.CooldownSec = request.CooldownSec ?? notification.CooldownSec;

        await _settingsRepository.UpsertNotificationSettingsAsync(notification, cancellationToken);

        return new NotificationSettingsDto
        {
            Id = notification.Id,
            OnDown = notification.OnDown,
            OnUp = notification.OnUp,
            OnLatency = notification.OnLatency,
            CooldownSec = notification.CooldownSec
        };
    }
}
