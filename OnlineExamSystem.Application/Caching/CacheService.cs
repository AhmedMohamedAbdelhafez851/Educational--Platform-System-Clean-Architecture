using Microsoft.Extensions.Caching.Memory;
using OnlineExamSystem.Application.Abstraction;

namespace OnlineExamSystem.Application.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // =========================================
        // GET
        // =========================================

        public Task<T?> GetAsync<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);

            return Task.FromResult(value);
        }

        // =========================================
        // SET
        // =========================================

        public Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiration ?? TimeSpan.FromMinutes(5)
            };

            _memoryCache.Set(key, value, options);

            return Task.CompletedTask;
        }

        // =========================================
        // REMOVE
        // =========================================

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public Task SetAsync<TResponse>(string cacheKey, TResponse? response, int slidingExpirationInMinutes)
        {
            throw new NotImplementedException();
        }
    }
}