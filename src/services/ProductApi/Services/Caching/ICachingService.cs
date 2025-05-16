using System.Text.Json;
using StackExchange.Redis;

namespace ProductApi.Services.Caching
{
    public interface ICachingService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
        Task RemoveAsync(string key);
        // متدهای دیگر مانند RemoveByPatternAsync ممکن است لازم شوند
    }

    public class RedisCachingService : ICachingService
    {
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<RedisCachingService> _logger;

        public RedisCachingService(IConnectionMultiplexer redisConnection, ILogger<RedisCachingService> logger)
        {
            _redisDatabase = redisConnection.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var serializedValue = await _redisDatabase.StringGetAsync(key);
                if (serializedValue.IsNullOrEmpty)
                {
                    return null;
                }
                return JsonSerializer.Deserialize<T>(serializedValue.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from Redis cache for key {CacheKey}", key);
                return null; // در صورت خطا، کش را نادیده بگیر
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _redisDatabase.StringSetAsync(key, serializedValue, absoluteExpirationRelativeToNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value to Redis cache for key {CacheKey}", key);
                // خطا در نوشتن به کش نباید عملیات اصلی را متوقف کند
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _redisDatabase.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing value from Redis cache for key {CacheKey}", key);
            }
        }
    }
}
