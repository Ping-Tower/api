namespace  Application.Common.Interfaces;

public interface IEmailService
{
    Task SendMessage(string email, string subject, string htmlbody, CancellationToken cancellationToken);
}