using FluentValidation;

namespace Application.Features.Auth.Commands.ResendVerificationCode;

public class ResendVerificationCodeCommandValidator : AbstractValidator<ResendVerificationCodeCommand>
{
    public ResendVerificationCodeCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
