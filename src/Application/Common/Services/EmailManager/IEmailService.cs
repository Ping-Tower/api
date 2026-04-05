namespace  Application.Common.Interfaces;

public interface IEmailService
{
    Task SendMessageAsync(
        string email,
        string templateId,
        IReadOnlyDictionary<string, string?> data,
        CancellationToken cancellationToken);
}
