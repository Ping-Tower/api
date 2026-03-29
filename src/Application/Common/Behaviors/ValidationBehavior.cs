using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validateOptions;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validateOptions)
    {
        _validateOptions = validateOptions;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validateOptions.Any())
        {
            var validationContext = new ValidationContext<TRequest>(request);

            var validationResult = await Task.WhenAll(_validateOptions.Select(s => s.ValidateAsync(validationContext, cancellationToken)));

            var failtures = validationResult.Where(w => w.Errors.Any()).SelectMany(w => w.Errors).ToList();

            if (failtures.Any())
                throw new ValidationException(failtures);
        }

        return await next();

    }
}