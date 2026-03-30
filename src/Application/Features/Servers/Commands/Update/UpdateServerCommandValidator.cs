using FluentValidation;

namespace Application.Features.Servers.Commands.Update;

public class UpdateServerCommandValidator : AbstractValidator<UpdateServerCommand>
{
    public UpdateServerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Host).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Query).MaximumLength(255);
        RuleFor(x => x.Port).InclusiveBetween(1, 65535);
        RuleFor(x => x.Protocol).IsInEnum();
    }
}
