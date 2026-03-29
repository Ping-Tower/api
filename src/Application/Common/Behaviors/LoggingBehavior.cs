using System.Reflection.Metadata;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;

        try
        {
            _logger.Log(LogLevel.Information, $"Start executing {name}, at {DateTime.UtcNow}");
            return await next();
        }
        catch
        {
            _logger.Log(LogLevel.Error, $"Error executing {name}, at {DateTime.UtcNow}");
            throw;
        }
    }

}