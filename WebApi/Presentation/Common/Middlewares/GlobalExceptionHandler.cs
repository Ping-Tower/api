using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace coo.Presentation.Common.Middlewares;

public static class GlobalExceptionHandler
{
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                if (feature is null)
                    return;

                var exception = feature.Error;

                var problem = exception switch
                {
                    Domain.Common.Exceptions.DomainException e => new ProblemDetails
                    {
                        Title = "Domain Exception",
                        Status = Status400BadRequest,
                        Detail = e.Message
                    },
                    Application.Common.Exceptions.ValidationException e => new ProblemDetails
                    {
                        Title = "Validation Exception",
                        Status = Status400BadRequest,
                        Detail = e.Message
                    },
                    Application.Common.Exceptions.NotFoundException e => new ProblemDetails
                    {
                        Title = "NotFound Exception",
                        Status = Status404NotFound,
                        Detail = e.Message
                    },
                    _ => new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Status = Status500InternalServerError,
                        Detail = "An unexpected error occurred."
                    }
                };

                context.Response.StatusCode = problem.Status ?? 500;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            });
        });

        return app;
    }
}