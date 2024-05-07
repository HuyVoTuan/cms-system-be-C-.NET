using Dummy.Infrastructure.Services.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime;

namespace Dummy.Infrastructure.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection ServicesConfiguration(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });
            services.AddMemoryCache();         
            return services;
        }
    }
}
