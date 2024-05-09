using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Dummy.Infrastructure.Extensions
{
    public static class QuartzExtension
    {
        public static IServiceCollection QuartzConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(opt =>
            {
                opt.UseTimeZoneConverter();
                opt.UseSimpleTypeLoader();
                opt.UsePersistentStore(_opt =>
                {
                    _opt.UseClustering();
                    _opt.UsePostgres(config =>
                    {
                        config.ConnectionString = configuration.GetConnectionString("MainDbContext");
                        config.TablePrefix = "quartz.qrtz_";
                    });
                    _opt.UseNewtonsoftJsonSerializer();
                });
                opt.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 5;
                });
            });

            return services;
        }
    }
}
