namespace LinkUp.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
