using Dummy.Infrastructure.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Dummy.Infrastructure.Middlewares
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public LocalizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var language = context.Request.Headers["Accept-Language"].ToString();

            var jsonLocalizationOptions = _configuration.GetSection(nameof(JsonLocalizationOptions)).Get<JsonLocalizationOptions>();
            var supportedCultureInfos = jsonLocalizationOptions.SupportedCultureInfos.ToList();

            var cultureKey = supportedCultureInfos.FirstOrDefault(x => x.StartsWith(language));

            if (cultureKey is null)
            {
                cultureKey = jsonLocalizationOptions.DefaultCulture;
            }

            var applicationCulture = new CultureInfo(cultureKey);
            Thread.CurrentThread.CurrentCulture = applicationCulture;
            Thread.CurrentThread.CurrentUICulture = applicationCulture;

            await _next(context);
        }
    }
}
