using Infrastructure.RedisManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Api.IntegrationTests;

public class RedisBackplaneTests
{
    [Fact]
    public async Task Broadcast_ReachesClientsOnBothReplicas()
    {
        var prefix = $"backplane-test-{Guid.NewGuid():N}";
        await using var first = CreateReplica(prefix);
        await using var second = CreateReplica(prefix);
        await first.StartAsync();
        await second.StartAsync();

        await using var firstClient = CreateClient(first);
        await using var secondClient = CreateClient(second);
        var firstReceived = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondReceived = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        firstClient.On<string>("status", value => firstReceived.TrySetResult(value));
        secondClient.On<string>("status", value => secondReceived.TrySetResult(value));
        await firstClient.StartAsync();
        await secondClient.StartAsync();

        var expected = Guid.NewGuid().ToString();
        var hub = first.Services.GetRequiredService<IHubContext<BackplaneTestHub>>();
        await hub.Clients.All.SendAsync("status", expected);

        Assert.Equal(expected, await firstReceived.Task.WaitAsync(TimeSpan.FromSeconds(10)));
        Assert.Equal(expected, await secondReceived.Task.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    private static WebApplication CreateReplica(string prefix)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RedisSettings:Configuration"] = Environment.GetEnvironmentVariable("TEST_REDIS_CONNECTION_STRING") ?? "localhost:6379",
            ["RedisSettings:KeyPrefix"] = prefix
        });
        builder.Services.ApplyRedis(builder.Configuration);
        var app = builder.Build();
        app.MapHub<BackplaneTestHub>("/hub");
        return app;
    }

    private static HubConnection CreateClient(WebApplication app) => new HubConnectionBuilder()
        .WithUrl($"{app.Urls.Single()}/hub", options =>
        {
            options.Transports = HttpTransportType.WebSockets;
            options.SkipNegotiation = true;
        })
        .Build();

    public class BackplaneTestHub : Hub;
}
