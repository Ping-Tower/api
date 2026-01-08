using Infrastructure.DataManager.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.IdentityManager.AspNetCoreIdentity;

namespace Infrastructure.Identity.AspNetCoreIdentity;

public static class DI
{
    public static IServiceCollection ApplyIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var nameSectionOnSettings = "AspNetIdentity";

        services.Configure<IdentitySettings>(configuration.GetSection(nameSectionOnSettings));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            var identitySettings = configuration.GetSection(nameSectionOnSettings).Get<IdentitySettings>();

            if (identitySettings == null)
            {
                throw new Exception($"{identitySettings} section on settings is missing!");
            }

            //Password
            options.Password.RequireDigit = identitySettings.Password.RequireDigit;
            options.Password.RequiredLength = identitySettings.Password.RequiredLenght;
            options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
            options.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;

            //Lockout
            options.Lockout.AllowedForNewUsers = identitySettings.Lockout.AllowedForNewUsers;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identitySettings.Lockout.DefaultLockoutTimespanInMinutes);
            options.Lockout.MaxFailedAccessAttempts = identitySettings.Lockout.MaxFailedAccessAttempts;

            //User
            options.User.RequireUniqueEmail = identitySettings.User.RequireUniqueEmail;

            //SignIn
            options.SignIn.RequireConfirmedEmail = identitySettings.SignIn.RequireConfirmedEmail;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddRoles<IdentityRole>();

        



        return services;
    }
}