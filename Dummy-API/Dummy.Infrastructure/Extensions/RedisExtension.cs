using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Dummy.Infrastructure.Extensions
{
    public static class RedisExtension
    {
        public static IServiceCollection RedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:Url"];
                options.InstanceName = configuration["Redis:Prefix"];
            });
            return services;
        }
    }
}
