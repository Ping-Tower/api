using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Domain.Common.Exceptions;

namespace Infrastructure.IdentityManager.Tokens;

public static class DI
{
    public static IServiceCollection ApplyTokenManager(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenSettings = configuration.GetSection("JwtSettings").Get<TokenSettings>() ?? new TokenSettings();

        services.Configure<TokenSettings>(configuration.GetSection("JwtSettings"));
        services.AddSingleton(tokenSettings);

        services.AddAuthentication(options => options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenSettings.Issuer,
                ValidAudience = tokenSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey ?? throw new DomainException())),
                ClockSkew = TimeSpan.FromMinutes(tokenSettings.ExpireInMinute)
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddTransient<TokenService>();

        services.AddTransient<ClaimService>();

        return services;
    } 
}
