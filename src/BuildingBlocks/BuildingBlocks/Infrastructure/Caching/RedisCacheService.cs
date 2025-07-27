using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Caching
{
    /// <summary>
    /// A Redis-based implementation of the distributed cache service.
    /// </summary>
    public class RedisCacheService : IDistributedCacheService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (!value.HasValue)
                {
                    return null;
                }
                return JsonSerializer.Deserialize<T>(value!, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from Redis cache for key {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, _jsonSerializerOptions);
                await _database.StringSetAsync(key, serializedValue, absoluteExpirationRelativeToNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in Redis cache for key {Key}", key);
            }
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _database.KeyDeleteAsync(key);
        }

        // Implement other methods if needed...
        public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
            => _database.StringGetAsync(key).ContinueWith(t => t.Result.ToString(), cancellationToken);

        public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
            => _database.StringSetAsync(key, value, absoluteExpirationRelativeToNow);

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
            => _database.KeyExistsAsync(key);

        public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
            => _database.KeyTouchAsync(key);
    }
}
