using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Infrastructure.DataManager.Contexts;

namespace Infrastructure.DataManager;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
            var database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "pingtower_db";
            var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "123";

            connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
        }

        optionsBuilder.UseNpgsql(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
