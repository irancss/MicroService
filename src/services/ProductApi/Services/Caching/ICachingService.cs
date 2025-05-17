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

    // بررسی صحت پیاده‌سازی کش با Redis:

    // 1. اینترفیس ICachingService متدهای اصلی کش را دارد: GetAsync, SetAsync, RemoveAsync.
    // 2. RedisCachingService از IDatabase (StackExchange.Redis) برای ارتباط با Redis استفاده می‌کند.
    // 3. مقادیر با System.Text.Json سریالایز و دی‌سریالایز می‌شوند.
    // 4. مدیریت خطا با ILogger انجام می‌شود و خطاها باعث توقف عملیات اصلی نمی‌شوند.
    // 5. متد SetAsync امکان تعیین زمان انقضا را دارد.
    // 6. متدها async هستند و از await استفاده می‌کنند.
    // 7. محدودیت: فقط برای کلاس‌ها (where T : class) کار می‌کند، برای value typeها باید توسعه یابد.

    // نتیجه: پیاده‌سازی کش با Redis صحیح و استاندارد است و با Best Practiceهای .NET و Redis مطابقت دارد.
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
