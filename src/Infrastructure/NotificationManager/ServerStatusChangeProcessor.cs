using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Infrastructure.NotificationManager;

public class ServerStatusChangeProcessor : IServerStatusChangeProcessor
{
    private readonly INotificationContextRepository _notificationContextRepository;
    private readonly INotificationCooldownStore _notificationCooldownStore;
    private readonly IEmailService _emailService;
    private readonly ITelegramNotificationService _telegramNotificationService;
    private readonly IServerStatusNotifier _serverStatusNotifier;
    private readonly AppLinksSettings _appLinksSettings;
    private readonly ILogger<ServerStatusChangeProcessor> _logger;

    public ServerStatusChangeProcessor(
        INotificationContextRepository notificationContextRepository,
        INotificationCooldownStore notificationCooldownStore,
        IEmailService emailService,
        ITelegramNotificationService telegramNotificationService,
        IServerStatusNotifier serverStatusNotifier,
        AppLinksSettings appLinksSettings,
        ILogger<ServerStatusChangeProcessor> logger)
    {
        _notificationContextRepository = notificationContextRepository;
        _notificationCooldownStore = notificationCooldownStore;
        _emailService = emailService;
        _telegramNotificationService = telegramNotificationService;
        _serverStatusNotifier = serverStatusNotifier;
        _appLinksSettings = appLinksSettings;
        _logger = logger;
    }

    public async Task ProcessAsync(string serverId, ServerStatus status, CancellationToken cancellationToken)
    {
        var context = await _notificationContextRepository.GetServerNotificationContextAsync(serverId, cancellationToken);
        if (context is null)
        {
            _logger.LogWarning("Notification context not found. ServerId: {ServerId}, Status: {Status}", serverId, status);
            return;
        }

        await _serverStatusNotifier.NotifyStatusChangedAsync(context.UserId, serverId, status, cancellationToken);

        if (status is not (ServerStatus.DOWN or ServerStatus.UP))
            return;

        if (!ShouldSendForStatus(context, status))
            return;

        if (!HasAnyDestination(context))
            return;

        if (await _notificationCooldownStore.IsInCooldownAsync(serverId, status, cancellationToken))
        {
            _logger.LogInformation("Notification suppressed by cooldown. ServerId: {ServerId}, Status: {Status}", serverId, status);
            return;
        }

        var telegramText = BuildTelegramText(context, status);
        var telegramButtons = BuildTelegramButtons(context);

        if (context.EmailConfirmed && !string.IsNullOrWhiteSpace(context.Email))
        {
            await _emailService.SendMessageAsync(
                context.Email,
                BuildEmailTemplateId(status),
                BuildEmailData(context, status),
                cancellationToken);
        }

        foreach (var telegramUserId in context.TelegramUserIds)
        {
            await _telegramNotificationService.SendMessageAsync(
                telegramUserId,
                telegramText,
                telegramButtons,
                cancellationToken);
        }

        await _notificationCooldownStore.SetCooldownAsync(
            serverId,
            status,
            TimeSpan.FromSeconds(NormalizeCooldown(context.CooldownSec)),
            cancellationToken);
    }

    private static bool ShouldSendForStatus(ServerNotificationContextDto context, ServerStatus status)
    {
        return status switch
        {
            ServerStatus.DOWN => context.OnDown,
            ServerStatus.UP => context.OnUp,
            _ => false
        };
    }

    private static bool HasAnyDestination(ServerNotificationContextDto context)
    {
        return (context.EmailConfirmed && !string.IsNullOrWhiteSpace(context.Email)) ||
               context.TelegramUserIds.Count > 0;
    }

    private static string BuildEmailTemplateId(ServerStatus status)
    {
        return status switch
        {
            ServerStatus.DOWN => "server-down",
            ServerStatus.UP => "server-up",
            _ => "server-status"
        };
    }

    private static IReadOnlyDictionary<string, string?> BuildEmailData(ServerNotificationContextDto context, ServerStatus status)
    {
        return new Dictionary<string, string?>
        {
            ["serverName"] = DisplayName(context),
            ["host"] = string.IsNullOrWhiteSpace(context.Host) ? "unknown host" : context.Host,
            ["status"] = status.ToString(),
            ["timestampUtc"] = DateTimeOffset.UtcNow.ToString("u")
        };
    }

    private static int NormalizeCooldown(int cooldownSec) => cooldownSec >= 0 ? cooldownSec : 600;

    private string BuildTelegramText(ServerNotificationContextDto context, ServerStatus status)
    {
        var serverName = WebUtility.HtmlEncode(DisplayName(context));
        var serverSuffix = BuildHostSuffix(context);
        var timestampUtc = DateTimeOffset.UtcNow.ToString("HH:mm 'UTC'");

        return status switch
        {
            ServerStatus.DOWN =>
                $"🔴 <b>Внимание!</b>\nСервер <b>{serverName}</b>{serverSuffix} недоступен.\n\nВремя: {timestampUtc}",
            ServerStatus.UP =>
                $"🟢 <b>Сервер восстановлен</b>\nСервер <b>{serverName}</b>{serverSuffix} снова доступен.\n\nВремя: {timestampUtc}",
            _ =>
                $"ℹ️ <b>Статус сервера изменён</b>\nСервер <b>{serverName}</b>{serverSuffix} сменил статус на {WebUtility.HtmlEncode(status.ToString())}.\n\nВремя: {timestampUtc}"
        };
    }

    private IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>> BuildTelegramButtons(ServerNotificationContextDto context)
    {
        return
        [
            [
                new TelegramInlineButtonDto(
                    _appLinksSettings.TelegramDashboardButtonText,
                    Url: _appLinksSettings.BuildServerUrl(context.ServerId))
            ]
        ];
    }

    private static string DisplayName(ServerNotificationContextDto context)
    {
        if (!string.IsNullOrWhiteSpace(context.ServerName))
            return context.ServerName;

        if (!string.IsNullOrWhiteSpace(context.Host))
            return context.Host;

        return context.ServerId;
    }

    private static string BuildHostSuffix(ServerNotificationContextDto context)
    {
        if (string.IsNullOrWhiteSpace(context.Host))
            return string.Empty;

        return $" ({WebUtility.HtmlEncode(context.Host)})";
    }
}
