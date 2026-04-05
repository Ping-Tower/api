namespace Infrastructure.RedisManager;

public class RedisSettings
{
    public string Configuration { get; set; } = "localhost:6379";
    public string KeyPrefix { get; set; } = "api";
}
