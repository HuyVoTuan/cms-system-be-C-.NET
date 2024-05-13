namespace Dummy.Infrastructure.Services.EmailService
{
    public interface IEmailNotificationService
    {
        Task SendEmailAsync(string emailAddress, string emailEvent, List<string> subjects, List<string> contents);
        Task SendEmailAsync(string emailAddress, string emailEvent);
    }
}
