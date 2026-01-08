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
        services.Configure<TokenSettings>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options => options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var tokenSettings = configuration.GetSection("JwtSettings").Get<TokenSettings>();

            if(tokenSettings == null)
                throw new Exception("JwtSettings are missing!");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = tokenSettings?.Issuer,
                ValidAudience = tokenSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings?.SecretKey ?? throw new DomainException())),
                ClockSkew = TimeSpan.FromMinutes(tokenSettings?.ExpireInMinute ?? 60)
            };
        });

        services.AddTransient<TokenService>();

        services.AddTransient<ClaimService>();

        return services;
    } 
}