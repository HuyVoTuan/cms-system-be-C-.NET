namespace Dummy.Infrastructure.Services.EmailService
{
    public interface IEmailNotificationService
    {
        Task SendEmailAsync(string email, object data, string eventType);
    }
}
