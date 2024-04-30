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
            services.AddValidatorsFromAssembly(executingAssembly).AddFluentValidationAutoValidation();
            return services;
        }
    }
}
