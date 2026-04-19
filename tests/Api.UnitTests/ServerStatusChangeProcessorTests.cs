using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Enums;
using Infrastructure.NotificationManager;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Api.UnitTests;

public class ServerStatusChangeProcessorTests
{
    [Fact]
    public async Task ProcessAsync_DownStatus_SendsEmailAndTelegramAndSetsCooldown()
    {
        var context = new ServerNotificationContextDto
        {
            ServerId = "server-1",
            UserId = "user-1",
            ServerName = "Primary DB",
            Host = "10.0.0.5",
            Email = "ops@example.com",
            EmailConfirmed = true,
            OnDown = true,
            OnUp = true,
            CooldownSec = 90,
            TelegramUserIds = [101, 202]
        };

        var repository = new FakeNotificationContextRepository(context);
        var cooldownStore = new FakeNotificationCooldownStore(isInCooldown: false);
        var emailService = new FakeEmailService();
        var telegramService = new FakeTelegramNotificationService();
        var statusNotifier = new FakeServerStatusNotifier();
        var settings = new AppLinksSettings
        {
            WebAppUrl = "https://app.example.com",
            ServerPathTemplate = "/servers/{serverId}",
            TelegramDashboardButtonText = "📊 Дашборд"
        };

        var processor = new ServerStatusChangeProcessor(
            repository,
            cooldownStore,
            emailService,
            telegramService,
            statusNotifier,
            settings,
            NullLogger<ServerStatusChangeProcessor>.Instance);

        await processor.ProcessAsync("server-1", ServerStatus.DOWN, CancellationToken.None);

        var notifiedStatus = Assert.Single(statusNotifier.Calls);
        Assert.Equal("user-1", notifiedStatus.UserId);
        Assert.Equal("server-1", notifiedStatus.ServerId);
        Assert.Equal(ServerStatus.DOWN, notifiedStatus.Status);

        var email = Assert.Single(emailService.Messages);
        Assert.Equal("ops@example.com", email.Email);
        Assert.Equal("server-down", email.TemplateId);
        Assert.Equal("Primary DB", email.Data["serverName"]);
        Assert.Equal("10.0.0.5", email.Data["host"]);
        Assert.Equal("DOWN", email.Data["status"]);
        Assert.True(email.Data.ContainsKey("timestampUtc"));

        Assert.Equal(2, telegramService.Messages.Count);
        foreach (var message in telegramService.Messages)
        {
            Assert.Contains("🔴 <b>Внимание!</b>", message.Text);
            Assert.Contains("Сервер <b>Primary DB</b> (10.0.0.5) недоступен.", message.Text);
            Assert.Contains("Время: ", message.Text);

            var row = Assert.Single(message.InlineButtons!);
            var button = Assert.Single(row);
            Assert.Equal("📊 Дашборд", button.Text);
            Assert.Equal("https://app.example.com/servers/server-1", button.Url);
            Assert.Null(button.CallbackData);
        }

        var cooldown = Assert.Single(cooldownStore.SetCalls);
        Assert.Equal("server-1", cooldown.ServerId);
        Assert.Equal(ServerStatus.DOWN, cooldown.Status);
        Assert.Equal(TimeSpan.FromSeconds(90), cooldown.Ttl);
    }

    [Fact]
    public async Task ProcessAsync_WhenCooldownIsActive_SkipsOutboundNotifications()
    {
        var context = new ServerNotificationContextDto
        {
            ServerId = "server-1",
            UserId = "user-1",
            ServerName = "Primary DB",
            Host = "10.0.0.5",
            Email = "ops@example.com",
            EmailConfirmed = true,
            OnDown = true,
            OnUp = true,
            CooldownSec = 90,
            TelegramUserIds = [101]
        };

        var repository = new FakeNotificationContextRepository(context);
        var cooldownStore = new FakeNotificationCooldownStore(isInCooldown: true);
        var emailService = new FakeEmailService();
        var telegramService = new FakeTelegramNotificationService();
        var statusNotifier = new FakeServerStatusNotifier();
        var settings = new AppLinksSettings
        {
            WebAppUrl = "https://app.example.com"
        };

        var processor = new ServerStatusChangeProcessor(
            repository,
            cooldownStore,
            emailService,
            telegramService,
            statusNotifier,
            settings,
            NullLogger<ServerStatusChangeProcessor>.Instance);

        await processor.ProcessAsync("server-1", ServerStatus.DOWN, CancellationToken.None);

        var notifiedStatus = Assert.Single(statusNotifier.Calls);
        Assert.Equal("user-1", notifiedStatus.UserId);
        Assert.Equal("server-1", notifiedStatus.ServerId);
        Assert.Equal(ServerStatus.DOWN, notifiedStatus.Status);
        Assert.Empty(emailService.Messages);
        Assert.Empty(telegramService.Messages);
        Assert.Empty(cooldownStore.SetCalls);
    }

    private sealed class FakeNotificationContextRepository(ServerNotificationContextDto? context) : INotificationContextRepository
    {
        public Task<ServerNotificationContextDto?> GetServerNotificationContextAsync(string serverId, CancellationToken cancellationToken) =>
            Task.FromResult(context);
    }

    private sealed class FakeNotificationCooldownStore(bool isInCooldown) : INotificationCooldownStore
    {
        public List<CooldownSetCall> SetCalls { get; } = [];

        public Task<bool> IsInCooldownAsync(string serverId, ServerStatus status, CancellationToken cancellationToken) =>
            Task.FromResult(isInCooldown);

        public Task SetCooldownAsync(string serverId, ServerStatus status, TimeSpan ttl, CancellationToken cancellationToken)
        {
            SetCalls.Add(new CooldownSetCall(serverId, status, ttl));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeEmailService : IEmailService
    {
        public List<SentEmail> Messages { get; } = [];

        public Task SendMessageAsync(
            string email,
            string templateId,
            IReadOnlyDictionary<string, string?> data,
            CancellationToken cancellationToken)
        {
            Messages.Add(new SentEmail(email, templateId, data));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTelegramNotificationService : ITelegramNotificationService
    {
        public List<SentTelegramMessage> Messages { get; } = [];

        public Task SendMessageAsync(
            long chatId,
            string text,
            IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons,
            CancellationToken cancellationToken)
        {
            Messages.Add(new SentTelegramMessage(chatId, text, inlineButtons));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeServerStatusNotifier : IServerStatusNotifier
    {
        public List<StatusNotification> Calls { get; } = [];

        public Task NotifyStatusChangedAsync(string userId, string serverId, ServerStatus status, CancellationToken cancellationToken)
        {
            Calls.Add(new StatusNotification(userId, serverId, status));
            return Task.CompletedTask;
        }
    }

    private sealed record CooldownSetCall(string ServerId, ServerStatus Status, TimeSpan Ttl);
    private sealed record SentEmail(string Email, string TemplateId, IReadOnlyDictionary<string, string?> Data);
    private sealed record SentTelegramMessage(
        long ChatId,
        string Text,
        IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? InlineButtons);
    private sealed record StatusNotification(string UserId, string ServerId, ServerStatus Status);
}
