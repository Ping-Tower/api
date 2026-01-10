namespace Infrastructure.RabbitMqManager;

public class RabbitMqSettings
{
    public string? HostName { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string MainQueue { get; set; } = null!;
    public string DlxName { get; set; } = null!;
    public string DlqName { get; set; } = null!;
}