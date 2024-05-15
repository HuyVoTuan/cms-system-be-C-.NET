namespace Dummy.Infrastructure.Services.EmailService
{
    public interface IEmailNotificationService
    {
        Task SendEmailAsync(String emailAddress, String emailEvent, List<String> subjects, List<String> contents);
        Task SendEmailAsync(String emailAddress, String emailEvent);
    }
}
