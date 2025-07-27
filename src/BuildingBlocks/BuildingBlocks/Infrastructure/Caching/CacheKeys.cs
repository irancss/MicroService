namespace BuildingBlocks.Infrastructure.Caching;

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