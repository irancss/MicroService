namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Interface for distributed caching operations across microservices.
/// </summary>
public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) where T : class;
    Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task RefreshAsync(string key, CancellationToken cancellationToken = default);
}

/// <summary>
/// Static class containing common cache key patterns for microservices
/// </summary>
public static class CacheKeys
{
    public static string Product(string id) => $"product:{id}";
    public static string AllProducts() => "products:all";
    public static string UserProfile(string userId) => $"user:profile:{userId}";
    public static string Cart(string userId) => $"cart:{userId}";
    public static string Order(string orderId) => $"order:{orderId}";
    public static string UserOrders(string userId) => $"orders:user:{userId}";
    public static string Configuration(string key) => $"config:{key}";
    public static string UserDashboard(string userId) => $"dashboard:user:{userId}";

}

