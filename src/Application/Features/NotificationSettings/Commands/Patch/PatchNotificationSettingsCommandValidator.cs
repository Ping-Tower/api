using FluentValidation;

namespace Application.Features.NotificationSettings.Commands.Patch;

public class PatchNotificationSettingsCommandValidator : AbstractValidator<PatchNotificationSettingsCommand>
{
    public PatchNotificationSettingsCommandValidator()
    {
        When(x => x.CooldownSec.HasValue, () => RuleFor(x => x.CooldownSec).GreaterThanOrEqualTo(0));
    }
}
