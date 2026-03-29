using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string type, string id) : base(string.Concat(type + id.ToString())) { }

    public NotFoundException(string type) : base(type) { }

    public NotFoundException() : base(CreateMessage()) { }

    public static string CreateMessage(string type = "", int id = 0)
    {
        string message = "The object was not found.";
        if (string.IsNullOrWhiteSpace(type))
            return message;

        var info = $"\nAdditional information: \n - type: {type}";

        if (id != 0)
            info += $"\n - id: {id}";

        return string.Concat(message, info);
    }
}