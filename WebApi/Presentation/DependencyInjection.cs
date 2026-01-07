using Presentation.Services;

namespace Presentation;

public static class DI
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.ApplyUserContext();

        return services;
    } 
}