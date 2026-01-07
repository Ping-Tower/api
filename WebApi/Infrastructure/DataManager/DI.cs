using Application.Common.Interfaces;
using Infrastructure.DataManager.Contexts;
using Infrastructure.DataManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.Replication;

namespace Infrastructure.DataManager;

public static class DI
{
    public static IServiceCollection ApplyDataManager(this IServiceCollection services, IConfiguration configuration)
    {
        var conectionstring = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(conectionstring));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}