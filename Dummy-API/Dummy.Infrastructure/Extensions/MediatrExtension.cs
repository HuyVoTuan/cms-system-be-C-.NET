using Dummy.Infrastructure.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dummy.Infrastructure.Extensions
{
    public static class MediatRExtension
    {
        public static IServiceCollection MediatRConfiguration(this IServiceCollection services)
        {
            var executingAssembly = AppDomain.CurrentDomain.Load("Dummy.Application");
            services.AddMediatR(cfg =>
                     cfg.RegisterServicesFromAssemblies(executingAssembly));

            // MediatR Pipelines
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(MediatRLoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));

            return services;
        }
    }
}
