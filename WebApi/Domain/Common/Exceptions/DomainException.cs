using System.Collections;
using System.Runtime.InteropServices;

namespace Domain.Common.Exceptions;
public class DomainException : Exception
{
    public IReadOnlyDictionary<string, IEnumerable<string>>? ValidationErrors;

    public DomainException() : this(null!) {}
    public DomainException(IReadOnlyDictionary<string, IEnumerable<string>> validationErrors) : base(CreateMessage(validationErrors))
    {
        ValidationErrors = validationErrors;
    }

    public DomainException(string subject, IEnumerable<string> validationErrors) : base(CreateMessage(new Dictionary<string, IEnumerable<string>> { {subject, validationErrors } }))
    {
        ValidationErrors = new Dictionary<string, IEnumerable<string>> { {subject, validationErrors } };
    }

    public static string CreateMessage(IReadOnlyDictionary<string, IEnumerable<string>>? validationErrors)
    {
        var baseMessage = "Validation error at the Domain level.";
        if (validationErrors is null || !validationErrors.Any())
            return baseMessage;

        var lines = validationErrors.Select(w =>
            $"- {w.Key}: {string.Join(", ", w.Value)}");

        return baseMessage + Environment.NewLine + string.Join(Environment.NewLine, lines);
    }
}