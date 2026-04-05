namespace Infrastructure.TelegramManager;

public class TelegramMessageDto
{
    public long ChatId { get; set; }
    public string Text { get; set; } = null!;
    public List<List<TelegramInlineButtonMessageDto>> InlineButtons { get; set; } = [];
}
