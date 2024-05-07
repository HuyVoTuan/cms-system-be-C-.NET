using Dummy.Infrastructure.Helpers;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Dummy.Infrastructure.Services.EmailService
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly EmailSetting _emailSetting;

        public EmailNotificationService(IOptions<EmailSetting> emailSetting)
        {
            _emailSetting = emailSetting.Value;
        }
        public async Task SendEmailAsync(string email, object data, string eventType)
        {
            await Send(new EmailContent()
            {
                To = email,
                Data = data,
                EventType = eventType
            });
        }

        private async Task Send(EmailContent emailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_emailSetting.DisplayName, _emailSetting.Mail);
            email.From.Add(new MailboxAddress(_emailSetting.DisplayName, _emailSetting.Mail));
            email.To.Add(MailboxAddress.Parse(emailContent.To));
            email.Subject = EmailHelper.GetEmailSubject(emailContent.EventType);

            var builder = new BodyBuilder();
            builder.HtmlBody = EmailHelper.GetEmailTemplate(emailContent.EventType, emailContent.Data);

            email.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect(_emailSetting.Host, _emailSetting.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_emailSetting.Mail, _emailSetting.Password);
                    await smtp.SendAsync(email);
                }
                catch (Exception ex)
                {
                    return;
                }

                smtp.Disconnect(true);
            }
        }
    }
}
