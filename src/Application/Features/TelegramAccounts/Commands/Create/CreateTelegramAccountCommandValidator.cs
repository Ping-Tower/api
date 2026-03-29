using FluentValidation;

namespace Application.Features.TelegramAccounts.Commands.Create;

public class CreateTelegramAccountCommandValidator : AbstractValidator<CreateTelegramAccountCommand>
{
    public CreateTelegramAccountCommandValidator()
    {
        RuleFor(x => x.TelegramUserId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Username).MaximumLength(255);
        RuleFor(x => x.PhotoUrl).MaximumLength(2048);
        RuleFor(x => x.AuthDate).GreaterThan(0);
        RuleFor(x => x.Hash).NotEmpty();
    }
}
