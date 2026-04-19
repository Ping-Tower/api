using FluentValidation;

namespace Application.Features.Pings.Queries.GetPings;

public class GetPingsQueryValidator : AbstractValidator<GetPingsQuery>
{
    public GetPingsQueryValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);

        When(x => x.From.HasValue && x.To.HasValue, () =>
        {
            RuleFor(x => x.To).GreaterThan(x => x.From);
        });
    }
}
