namespace Infrastructure.RabbitMqManager;

public class RabbitMqSettings
{
    public string? HostName { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string MainQueue { get; set; } = null!;
    public string ServerEventsExchange { get; set; } = null!;
    public string ServerAddedRoutingKey { get; set; } = null!;
    public string ServerEditedRoutingKey { get; set; } = null!;
    public string ServerDeletedRoutingKey { get; set; } = null!;
    public string ServerStatusChangedQueue { get; set; } = null!;
}
