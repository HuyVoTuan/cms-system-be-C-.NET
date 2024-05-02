using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Text.Json;


namespace Dummy.Infrastructure.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IMemoryCache _memoryCache;

        public JsonStringLocalizer(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => throw new NotImplementedException();

        // Access localizer through index _localizer["KeyExample"]
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value, string.IsNullOrEmpty(value));
            }
        }
        private string GetString(string key)
        {
            // Get resources path
            var infrastructureAssembly = Assembly.GetExecutingAssembly();
            var resource = $"{infrastructureAssembly.GetName().Name}.Localization.Resources.{Thread.CurrentThread.CurrentCulture.Name}.json";

            // <summary>
            // Setup cache key in order to save server performance

            // 1.) What if 1000 users make each request to server and facing error messages
            //     at their request which caught by Fluent Validaiton pipeline. 
            //     => Server have to perform read resoures file for 1000 times 
            //     => Server might die

            // 2.) Caching is the way to fix the problem.
            //     Cache the keys whenever Fluent Validaiton throw error messages
            //     Perform multi language mechanism (Localizer)
            //     Finish caching.

            var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";

            if (_memoryCache.TryGetValue(cacheKey, out String cacheValue))
            {
                return cacheValue;
            }

            // <summary>
            // Create a stream and read through entire resoure file with the desire resource file path.
            // In order to read through JSON file using GetManifestResourceStream
            // If not configure JSON file as embedded resource, possibiblity getting null when using GetManifestResourceStream is 100%

            // 1.) Create JSON file at target location
            // 2.) Get resource path base on personal logic
            // 3.) Perform Embedded JSON file:
            //     3.1) Right click JSON file -> Choose Properties
            //     3.2) Set "Build Action" to "Embedded Resource"

            using (Stream resourceStream = infrastructureAssembly.GetManifestResourceStream(resource))
            {
                if (resourceStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(resourceStream))
                    {
                        using (var jsonDocument = JsonDocument.Parse(streamReader.ReadToEnd()))
                        {
                            var keys = key.Split(".");

                            // Representing the entire JSON value at the root level of the document.
                            JsonElement currentElement = jsonDocument.RootElement;

                            foreach (var part in keys)
                            {
                                if (currentElement.TryGetProperty(part, out JsonElement value))
                                {
                                    currentElement = value;
                                }
                                else
                                {
                                    return String.Empty;
                                }
                            }

                            var finalValue = currentElement.GetString();

                            _memoryCache.Set(cacheKey, finalValue, TimeSpan.FromMinutes(10));

                            return finalValue;
                        }
                    }
                }

                return String.Empty;
            }
        }
    }
}
