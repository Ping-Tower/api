namespace Infrastructure.TelegramManager;

public class TelegramInlineButtonMessageDto
{
    public string Text { get; set; } = null!;
    public string? Url { get; set; }
    public string? CallbackData { get; set; }
}
