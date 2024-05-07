using Dummy.Infrastructure.Services.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Dummy.Infrastructure.Extensions
{
    public static class EmailNotificationExtension
    {
        public static IServiceCollection EmailNotificationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<EmailSetting>(otp => configuration.GetSection("EmailSetting").Bind(otp));
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            return services;
        }
    }
}
