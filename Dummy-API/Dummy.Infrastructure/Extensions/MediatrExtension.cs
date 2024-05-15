using Dummy.Infrastructure.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Dummy.Infrastructure.Extensions
{
    public static class MediatRExtension
    {
        public static IServiceCollection MediatRConfiguration(this IServiceCollection services)
        {
            var executingAssembly = AppDomain.CurrentDomain.Load("Dummy.Application");
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(executingAssembly);
                // MediatR Pipelines
                cfg.AddOpenBehavior(typeof(MediatRLoggingBehavior<,>), ServiceLifetime.Singleton);
            });


            return services;
        }
    }
}
