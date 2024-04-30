using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace Dummy.Infrastructure.Extensions
{
    public static class RedisExtension
    {
        public static IServiceCollection RedisConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            var configurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = true,
                AllowAdmin = true,
                ConnectRetry = 5,
                SyncTimeout = 5000,
            };

            configurationOptions.EndPoints.Add(configuration["Redis:Connection"]);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
                options.InstanceName = configuration["Redis:InstanceName"];
            });
            return services;
        }
    }
}
