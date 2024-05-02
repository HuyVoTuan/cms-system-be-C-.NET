using Dummy.Infrastructure.Services.EmailService;
using Microsoft.Extensions.DependencyInjection;


namespace Dummy.Infrastructure.Extensions
{
    public static class EmailNotificationExtension
    {
        public static IServiceCollection EmailNotificationConfiguration(this IServiceCollection services)
        {
            services.AddOptions();         
            services.AddTransient<IEmailNotificationService, EmailNotificationService>();
            return services;
        }
    }
}
