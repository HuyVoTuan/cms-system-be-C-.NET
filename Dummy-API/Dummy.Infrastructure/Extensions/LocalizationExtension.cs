using Dummy.Infrastructure.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Dummy.Infrastructure.Extensions
{
    public static class LocalizationExtension
    {
        public static IServiceCollection LocalizationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();

            var jsonLocalizationOptions = configuration.GetSection(nameof(JsonLocalizationOptions)).Get<JsonLocalizationOptions>();
            var supportedCultureInfos = jsonLocalizationOptions.SupportedCultureInfos.ToList();

            services.Configure<RequestLocalizationOptions>(opt =>
            {
                opt.SetDefaultCulture(jsonLocalizationOptions.DefaultCulture);
                opt.SupportedCultures = supportedCultureInfos.Select(x => new CultureInfo(x)).ToList();
            });

            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddScoped<IStringLocalizer, JsonStringLocalizer>();
            return services;
        }

        public static String Translate(this IStringLocalizer<dynamic> localizer, String key)
        {
            var result = localizer[key.ToLower()];
            return !result.ResourceNotFound ? result.Value : key;
        }

        public static String Translate(this IStringLocalizer<dynamic> localizer, String key, List<String> args)
        {
            var result = localizer[key.ToLower(), args.ToArray()];
            return !result.ResourceNotFound ? result.Value : key;
        }
    }
}
