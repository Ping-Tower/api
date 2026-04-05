using Infrastructure.DataManager.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.IdentityManager.AspNetCoreIdentity;

namespace Infrastructure.IdentityManager.AspNetCoreIdentity;

public static class DI
{
    public static IServiceCollection ApplyIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var identitySectionName = "AspNetIdentity";
        var authLinkSectionName = "AuthLinkSettings";
        var authEmailRateLimitSectionName = "AuthEmailRateLimitSettings";

        var identitySettings = configuration.GetSection(identitySectionName).Get<IdentitySettings>() ?? new IdentitySettings();
        var authLinkSettings = configuration.GetSection(authLinkSectionName).Get<AuthLinkSettings>() ?? new AuthLinkSettings();
        var authEmailRateLimitSettings = configuration.GetSection(authEmailRateLimitSectionName).Get<AuthEmailRateLimitSettings>() ?? new AuthEmailRateLimitSettings();

        services.Configure<IdentitySettings>(configuration.GetSection(identitySectionName));
        services.Configure<AuthLinkSettings>(configuration.GetSection(authLinkSectionName));
        services.Configure<AuthEmailRateLimitSettings>(configuration.GetSection(authEmailRateLimitSectionName));
        services.AddSingleton(identitySettings);
        services.AddSingleton(authLinkSettings);
        services.AddSingleton(authEmailRateLimitSettings);

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            //Password
            options.Password.RequireDigit = identitySettings.Password.RequireDigit;
            options.Password.RequiredLength = identitySettings.Password.RequiredLength;
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
        .AddRoles<IdentityRole>()
        .AddDefaultTokenProviders();

        return services;
    }
}
