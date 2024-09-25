using Microsoft.Extensions.Caching.Memory;

namespace FitnessWorkout.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetOrCreate<T>(string key, Func<T> createItem, TimeSpan? absoluteExpiration = null)
        {
            if (!_cache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = createItem();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Optional: Set sliding expiration

                if (absoluteExpiration.HasValue)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(absoluteExpiration.Value);
                }

                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
