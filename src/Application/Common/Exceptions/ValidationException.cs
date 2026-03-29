using System.Collections;
using System.Runtime.InteropServices;
using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationFailure>? ValidationErrors;
    public ValidationException(IEnumerable<ValidationFailure>? failures = null!) : base(CreateMessage(failures))
    {
        ValidationErrors = failures;
    }

    public static string CreateMessage(IEnumerable<ValidationFailure>? validationErrors)
    {
        if (validationErrors is null || !validationErrors.Any())
            return "Validation error.";

        var lines = validationErrors.Select(w =>
            $"- {w.PropertyName}: {w.ErrorMessage}");

        return string.Concat("Validation error at the Domain level.", string.Join(Environment.NewLine, lines));
    }
}