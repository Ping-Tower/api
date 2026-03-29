using FluentValidation;

namespace Application.Features.Settings.Commands.Update;

public class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsCommandValidator()
    {
        When(x => x.IntervalSec.HasValue, () => RuleFor(x => x.IntervalSec).GreaterThan(0));
        When(x => x.LatencyThresholdMs.HasValue, () => RuleFor(x => x.LatencyThresholdMs).GreaterThan(0));
        When(x => x.Retries.HasValue, () => RuleFor(x => x.Retries).GreaterThanOrEqualTo(0));
        When(x => x.FailureThreshold.HasValue, () => RuleFor(x => x.FailureThreshold).GreaterThan(0));
    }
}
