using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dummy.Infrastructure.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection DatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MainDBContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("MainDbContext")));
            return services;
        }
    }
}
