using FluentValidation;

namespace Application.Features.TelegramAccounts.Commands.Create;

public class CreateTelegramAccountCommandValidator : AbstractValidator<CreateTelegramAccountCommand>
{
    public CreateTelegramAccountCommandValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
    }
}
