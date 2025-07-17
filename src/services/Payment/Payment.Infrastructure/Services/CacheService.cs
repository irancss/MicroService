using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Payment.Infrastructure.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null);
    Task<string?> GetStringAsync(string key);
}

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
                return null;

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache value for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(key, json, expiration);
            
            _logger.LogDebug("Cache value set for key: {Key}, Expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache value removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache value for key: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null)
    {
        try
        {
            await _database.StringSetAsync(key, value, expiration);
            _logger.LogDebug("Cache string value set for key: {Key}, Expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache string value for key: {Key}", key);
        }
    }

    public async Task<string?> GetStringAsync(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache string value for key: {Key}", key);
            return null;
        }
    }
}

// Helper class for caching transaction states during payment process
public static class CacheKeys
{
    public static string TransactionState(string orderId) => $"transaction:state:{orderId}";
    public static string PaymentAttempt(string orderId) => $"payment:attempt:{orderId}";
    public static string GatewayResponse(string gatewayTransactionId) => $"gateway:response:{gatewayTransactionId}";
    public static string UserWallet(Guid userId) => $"wallet:user:{userId}";
}

public class TransactionCacheData
{
    public Guid TransactionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string GatewayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? GatewayTransactionId { get; set; }
}
