using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Services.IdentityManager;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Infrastructure.RabbitMqManager;

namespace Api.IntegrationTests;

public sealed class PingTowerApiFactory : WebApplicationFactory<Program>
{
    private static readonly IReadOnlyDictionary<string, string?> TestConfiguration = new Dictionary<string, string?>
    {
        ["ConnectionStrings:Postgres"] = Environment.GetEnvironmentVariable("TEST_POSTGRES_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=pingtower_test;Username=postgres;Password=test",
        ["AspNetIdentity:Password:RequireDigit"] = "true",
        ["AspNetIdentity:Password:RequireLowercase"] = "true",
        ["AspNetIdentity:Password:RequireUppercase"] = "true",
        ["AspNetIdentity:Password:RequireNonAlphanumeric"] = "false",
        ["AspNetIdentity:Password:RequiredLength"] = "8",
        ["AspNetIdentity:Lockout:DefaultLockoutTimespanInMinutes"] = "5",
        ["AspNetIdentity:Lockout:MaxFailedAccessAttempts"] = "5",
        ["AspNetIdentity:Lockout:AllowedForNewUsers"] = "true",
        ["AspNetIdentity:User:RequireUniqueEmail"] = "true",
        ["AspNetIdentity:SignIn:RequireConfirmedEmail"] = "true",
        ["JwtSettings:SecretKey"] = "12345678901234567890123456789012",
        ["JwtSettings:Audience"] = "PingTowerClient",
        ["JwtSettings:Issuer"] = "PingTowerApi",
        ["JwtSettings:ExpireInMinute"] = "60",
        ["JwtSettings:RefreshTokenExpireInDays"] = "7",
        ["TelegramAuthSettings:BotToken"] = "test-bot-token",
        ["TelegramAuthSettings:AuthLifetimeMinutes"] = "10",
        ["ClickHouseSettings:ConnectionString"] = "Host=localhost;Protocol=http;Port=8123;Username=default;Password=test;Database=pingtower_analytics",
        ["RabbitMqSettings:HostName"] = "localhost",
        ["RabbitMqSettings:Port"] = "5672",
        ["RabbitMqSettings:UserName"] = "guest",
        ["RabbitMqSettings:Password"] = "guest",
        ["RabbitMqSettings:MainQueue"] = "emailQueue",
        ["RabbitMqSettings:TelegramQueue"] = "telegramQueue",
        ["RabbitMqSettings:ServerEventsExchange"] = "serverEventsExchange",
        ["RabbitMqSettings:ServerAddedRoutingKey"] = "server.target.added",
        ["RabbitMqSettings:ServerUpdatedRoutingKey"] = "server.target.updated",
        ["RabbitMqSettings:ServerDeletedRoutingKey"] = "server.target.deleted",
        ["RabbitMqSettings:ServerStatusChangedQueue"] = "q.api.status-events",
        ["RedisSettings:Configuration"] = "localhost:6379",
        ["RedisSettings:KeyPrefix"] = "api",
        ["AppLinksSettings:WebAppUrl"] = "https://app.example.com",
        ["AppLinksSettings:ServerPathTemplate"] = "/servers/{serverId}",
        ["AppLinksSettings:TelegramDashboardButtonText"] = "📊 Дашборд"
    };

    public StubSender Sender { get; } = new();

    public PingTowerApiFactory()
    {
        foreach (var pair in TestConfiguration)
        {
            Environment.SetEnvironmentVariable(
                pair.Key.Replace(":", "__", StringComparison.Ordinal),
                pair.Value);
        }
    }

    public HttpClient CreateHttpsClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(TestConfiguration);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ISender>();
            services.AddSingleton<ISender>(Sender);

            services.RemoveAll<IIdentityService>();
            services.AddSingleton<IIdentityService, NoOpIdentityService>();
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService, NoOpEmailService>();
            services.RemoveAll<ITelegramNotificationService>();
            services.AddSingleton<ITelegramNotificationService, NoOpTelegramNotificationService>();
            services.RemoveAll<IServerEventPublisher>();
            services.AddSingleton<IServerEventPublisher, NoOpServerEventPublisher>();
            services.RemoveAll<RabbitMqHostedService>();
            services.RemoveAll<IRabbitMqProvider>();
            RemoveHostedServiceDescriptors(services);
        });
    }

    private static void RemoveHostedServiceDescriptors(IServiceCollection services)
    {
        var descriptors = services
            .Where(descriptor => descriptor.ServiceType == typeof(IHostedService) &&
                                 (descriptor.ImplementationType == typeof(ServerStatusRabbitMqWorker) ||
                                  descriptor.ImplementationFactory is not null))
            .ToList();

        foreach (var descriptor in descriptors)
            services.Remove(descriptor);
    }

    private sealed class NoOpIdentityService : IIdentityService
    {
        public Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<RegistrationResultDto> RegistrationAsync(string email, string password, string name, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task LogoutAsync(string refreshToken, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<RefreshResultDto> Refresh(string refreshToken, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<LoginResultDto> VerifyEmail(string email, string code, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<ForgotPasswordResultDto> ForgotPassword(string email, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<ResetPasswordResultDto> ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<ResendVerificationCodeResultDto> ResetVerifyEmailCode(string email, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class NoOpEmailService : IEmailService
    {
        public Task SendMessageAsync(string email, string templateId, IReadOnlyDictionary<string, string?> data, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class NoOpTelegramNotificationService : ITelegramNotificationService
    {
        public Task SendMessageAsync(
            long chatId,
            string text,
            IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class NoOpServerEventPublisher : IServerEventPublisher
    {
        public Task PublishServerAddedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task PublishServerEditedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task PublishServerDeletedAsync(Server server, PingSettings pingSettings, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
