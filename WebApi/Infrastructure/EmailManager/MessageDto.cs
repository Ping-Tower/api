namespace Infrastructure.EmailManager;

public class MessageDto
{
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }
}