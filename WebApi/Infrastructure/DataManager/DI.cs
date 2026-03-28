using Application.Common.Interfaces;
using Infrastructure.DataManager.Contexts;
using Infrastructure.DataManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataManager;

public static class DI
{
    public static IServiceCollection ApplyDataManager(this IServiceCollection services, IConfiguration configuration)
    {
        var conectionstring = configuration.GetConnectionString("Postgres");

        if(string.IsNullOrEmpty(conectionstring))
            throw new Exception("Postgres connectionstring is missing!");

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(conectionstring));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<IServerStateRepository, ServerStateRepository>();
        services.AddScoped<ITelegramAccountRepository, TelegramAccountRepository>();

        return services;
    }
}