namespace Infrastructure.EmailManager;

public class MessageDto
{
    public string? Email { get; set; }
    public string? TemplateId { get; set; }
    public Dictionary<string, string?> Data { get; set; } = new();
}
