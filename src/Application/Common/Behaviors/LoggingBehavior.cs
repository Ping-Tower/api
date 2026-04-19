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
            _logger.LogInformation("Start executing {RequestName}, at {Timestamp}", name, DateTime.UtcNow);
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {RequestName}, at {Timestamp}", name, DateTime.UtcNow);
            throw;
        }
    }

}