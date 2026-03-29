namespace Infrastructure.TelegramAuth;

public class TelegramAuthSettings
{
    public string BotToken { get; set; } = null!;
    public int AuthLifetimeMinutes { get; set; } = 10;
}
