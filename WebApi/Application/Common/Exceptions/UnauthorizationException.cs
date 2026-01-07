namespace Application.Common.Exceptions;

public class UnauthorizationException : Exception
{
    public UnauthorizationException(string message) : base(message) { }
}