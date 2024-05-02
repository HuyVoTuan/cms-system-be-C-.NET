using Dummy.Infrastructure.Services;
using Dummy.Infrastructure.Services.EmailService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
