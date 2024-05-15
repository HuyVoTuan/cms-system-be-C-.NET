using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dummy.Infrastructure.Extensions
{
    public static class FluentValidationExtension
    {
        public static IServiceCollection FluentValidationConfiguration(this IServiceCollection services)
        {
            var executingAssembly = AppDomain.CurrentDomain.Load("Dummy.Application");

            // Base validation base on Fluent Validation Behavior Pipeline
            services.AddValidatorsFromAssembly(executingAssembly).AddFluentValidationAutoValidation();

            return services;
        }
    }
}
