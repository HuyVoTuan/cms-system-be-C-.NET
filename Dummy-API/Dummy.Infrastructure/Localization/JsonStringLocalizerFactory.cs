using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;


namespace Dummy.Infrastructure.Localization
{
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IMemoryCache _memoryCache;

        public JsonStringLocalizerFactory(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_memoryCache);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_memoryCache);
        }
    }
}
