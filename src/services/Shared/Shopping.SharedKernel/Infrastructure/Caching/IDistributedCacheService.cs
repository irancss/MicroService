namespace Shopping.SharedKernel.Infrastructure.Caching;

/// <summary>
/// Interface for distributed caching operations across microservices
/// </summary>
public interface IDistributedCacheService
{
    /// <summary>
    /// Get a cached object by key
    /// </summary>
    /// <typeparam name="T">Type of the cached object</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached object or null if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Get a cached string value by key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached string or null if not found</returns>
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set an object in cache with optional expiration
    /// </summary>
    /// <typeparam name="T">Type of the object to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Object to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Set a string value in cache with optional expiration
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">String value to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove an item from cache by key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove multiple items from cache by pattern (Redis specific)
    /// </summary>
    /// <param name="pattern">Pattern to match keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a key exists in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if key exists, false otherwise</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh the expiration time of a cached item
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="expiration">New expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Static class containing common cache key patterns for microservices
/// </summary>
public static class CacheKeys
{
    // Product Service Cache Keys
    public static string Product(string id) => $"product:{id}";
    public static string ProductsByCategory(string categoryId) => $"products:category:{categoryId}";
    public static string ProductsByBrand(string brandId) => $"products:brand:{brandId}";
    public static string FeaturedProducts() => "products:featured";

    // User Service Cache Keys
    public static string User(string id) => $"user:{id}";
    public static string UserProfile(string userId) => $"user:profile:{userId}";
    public static string UserPreferences(string userId) => $"user:preferences:{userId}";

    // Cart Service Cache Keys
    public static string Cart(string userId) => $"cart:{userId}";
    public static string CartItemCount(string userId) => $"cart:count:{userId}";

    // Order Service Cache Keys
    public static string Order(string orderId) => $"order:{orderId}";
    public static string UserOrders(string userId) => $"orders:user:{userId}";
    public static string OrderStatus(string orderId) => $"order:status:{orderId}";

    // Inventory Service Cache Keys
    public static string ProductInventory(string productId) => $"inventory:product:{productId}";
    public static string InventoryByLocation(string locationId) => $"inventory:location:{locationId}";

    // Discount/Promotion Cache Keys
    public static string Discount(string discountCode) => $"discount:{discountCode}";
    public static string UserDiscounts(string userId) => $"discounts:user:{userId}";
    public static string ActivePromotions() => "promotions:active";

    // Configuration Cache Keys
    public static string Configuration(string key) => $"config:{key}";
    public static string FeatureFlags() => "features:flags";

    // Search Cache Keys
    public static string SearchResults(string query, int page = 1) => $"search:{query}:page:{page}";
    public static string PopularSearches() => "search:popular";

    // Session Cache Keys
    public static string UserSession(string sessionId) => $"session:{sessionId}";
    public static string TempData(string key) => $"temp:{key}";
}

/// <summary>
/// Cache configuration options
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Default cache expiration time
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Short-term cache expiration (for frequently changing data)
    /// </summary>
    public TimeSpan ShortTermExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Long-term cache expiration (for static data)
    /// </summary>
    public TimeSpan LongTermExpiration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Session expiration time
    /// </summary>
    public TimeSpan SessionExpiration { get; set; } = TimeSpan.FromMinutes(20);

    /// <summary>
    /// Whether to enable cache compression
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Cache key prefix for this service
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;
}