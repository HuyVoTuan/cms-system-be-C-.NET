using Dummy.Domain.Constants;
using Dummy.Infrastructure.Helpers;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MimeKit;

namespace Dummy.Infrastructure.Services.EmailService
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IStringLocalizer _localizer;
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IConfiguration configuration, IStringLocalizer localizer)
        {
            _localizer = localizer;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailAddress, string emailEvent, List<string> subjects, List<string> contents)
        {
            var emailSetting = _configuration.GetSection(nameof(EmailSetting)).Get<EmailSetting>();

            var email = new MimeMessage();
            email.Sender = new MailboxAddress(emailSetting.DisplayName, emailSetting.Mail);
            email.From.Add(new MailboxAddress(emailSetting.DisplayName, emailSetting.Mail));
            email.To.Add(MailboxAddress.Parse(emailAddress));

            var emailData = GetEmailData(emailEvent, subjects, contents);
            email.Subject = emailData.Subject;

            var builder = new BodyBuilder();
            builder.TextBody = emailData.Content;
            email.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect(emailSetting.Host, emailSetting.Port, SecureSocketOptions.StartTls);
                    smtp.Authenticate(emailSetting.Mail, emailSetting.Password);
                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }
                catch (Exception)
                {
                    smtp.Disconnect(true);
                    return;
                }

            }
        }

        public async Task SendEmailAsync(string emailAddress, string emailEvent)
        {
            await SendEmailAsync(emailAddress, emailEvent, null, null);
        }

        private EmailData GetEmailData(String emailEvent, List<String> subjects, List<String> contents)
        {
            if (!TypeHelper.GetConstants(typeof(EmailEvent)).Contains(emailEvent))
            {
                throw new Exception($"{emailEvent} is invalid email event");
            }

            return new EmailData
            {
                Subject = subjects is not null ? _localizer[$"notification.{emailEvent}.subject", subjects] : _localizer[$"notification.{emailEvent}.subject"],
                Content = contents is not null ? _localizer[$"notification.{emailEvent}.content", contents] : _localizer[$"notification.{emailEvent}.content"]
            };
        }
    }
}
