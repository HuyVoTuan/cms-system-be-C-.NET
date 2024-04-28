using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dummy.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }
        public T GetData<T>(string key)
        {
            try
            {
                var value = _cache.GetString(key);

                if (string.IsNullOrEmpty(value))
                {
                    _logger.LogWarning($"Key '{key}' not found in cache.");
                    return default;
                }

                return Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data for key: {key}", ex);
                return default;
            }
        }

        public object RemoveData(string key)
        {
            var result = true;

            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    _cache.Remove(key);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error removing data for key: {key}", ex);
                    result = false;
                }
            }
            else
            {
                _logger.LogError("Attempted to remove data with an empty or null key.");
                result = false;
            }

            return result;
        }

        public bool SetData<T>(string key, T value, TimeSpan expirationTime)
        {
            var result = true;

            if (string.IsNullOrEmpty(key) || value is null)
            {
                _logger.LogError("Attempted to set data with an empty or null key or value.");
                result = false;
            }
            else
            {
                try
                {
                    var timeOffset = DateTimeOffset.UtcNow.Add(expirationTime);
                    var serializedValue = Serialize<T>(value);

                    _cache.SetString(key, serializedValue, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = timeOffset,
                    });

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error setting data for key: {key}", ex);
                    return false;
                }
            }

            return result;
        }

        private T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        private string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}




