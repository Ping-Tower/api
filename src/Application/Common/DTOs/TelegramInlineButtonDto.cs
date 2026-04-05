namespace Application.Common.DTOs;

public record TelegramInlineButtonDto(
    string Text,
    string? Url = null,
    string? CallbackData = null);
