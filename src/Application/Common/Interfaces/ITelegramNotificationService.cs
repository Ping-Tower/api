using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface ITelegramNotificationService
{
    Task SendMessageAsync(
        long chatId,
        string text,
        IReadOnlyList<IReadOnlyList<TelegramInlineButtonDto>>? inlineButtons,
        CancellationToken cancellationToken);
}
