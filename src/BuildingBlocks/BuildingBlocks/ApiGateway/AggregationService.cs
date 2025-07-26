using BuildingBlocks.ApiGateway.Dtos;
using BuildingBlocks.Infrastructure.Caching;
using BuildingBlocks.ServiceMesh.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace BuildingBlocks.ApiGateway
{
    public interface IAggregationService
    {
        Task<DashboardData?> GetDashboardDataAsync(string userId);
        Task<OrderDetailsData?> GetOrderDetailsAsync(int orderId);
    }

    /// <summary>
    /// [نسخه صحیح] این پیاده‌سازی اکنون به درستی از کشینگ و تاب‌آوری برای تجمیع داده‌ها استفاده می‌کند.
    /// </summary>
    public class AggregationService : IAggregationService
    {
        private readonly IServiceMeshHttpClient _serviceMeshClient;
        private readonly IDistributedCacheService _cache;
        private readonly ILogger<AggregationService> _logger;

        public AggregationService(
            IServiceMeshHttpClient serviceMeshHttpClient,
            IDistributedCacheService cache,
            ILogger<AggregationService> logger)
        {
            _serviceMeshClient = serviceMeshHttpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<DashboardData?> GetDashboardDataAsync(string userId)
        {
            var cacheKey = CacheKeys.UserDashboard(userId);

            try
            {
                // 1. ابتدا تلاش برای خواندن از کش
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (cachedData != null)
                {
                    _logger.LogInformation("Dashboard data for user {UserId} found in cache.", userId);
                    return JsonSerializer.Deserialize<DashboardData>(cachedData);
                }

                _logger.LogInformation("Dashboard data for user {UserId} not in cache. Fetching from services.", userId);

                // 2. اگر در کش نبود، از سرویس‌ها بخوان
                var result = await FetchDashboardDataFromServices(userId);

                // 3. اگر نتیجه موفقیت‌آمیز بود، آن را در کش ذخیره کن
                if (result != null)
                {
                    var cacheExpiration = TimeSpan.FromSeconds(30);
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheExpiration);
                    _logger.LogInformation("Dashboard data for user {UserId} stored in cache.", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while aggregating dashboard data for user: {UserId}", userId);
                return null;
            }
        }

        private async Task<DashboardData?> FetchDashboardDataFromServices(string userId)
        {
            // فراخوانی‌های موازی برای سرویس‌های مختلف
            var userTask = GetJsonAsync<UserData>("user-service", $"/api/users/{userId}");
            var ordersTask = GetJsonAsync<List<OrderData>>("order-service", $"/api/orders/user/{userId}");

            // فراخوانی سرویس غیراساسی (توصیه‌ها) به صورت امن
            var recommendationsTask = GetRecommendationsSafeAsync(userId);

            await Task.WhenAll(userTask, ordersTask, recommendationsTask);

            var user = await userTask;
            if (user == null)
            {
                // داده‌های کاربر ضروری است. اگر وجود نداشته باشد، کل عملیات بی‌معنی است.
                _logger.LogWarning("Could not retrieve essential user data for user {UserId}. Aborting dashboard aggregation.", userId);
                return null;
            }

            var orders = await ordersTask;
            var recommendations = await recommendationsTask;

            return new DashboardData
            {
                User = user,
                RecentOrders = orders ?? new List<OrderData>(),
                Recommendations = recommendations // اگر خطا داده باشد، یک لیست خالی خواهد بود
            };
        }

        // یک متد کمکی برای فراخوانی امن سرویس توصیه‌ها (سرویس غیراساسی)
        private async Task<List<ProductData>> GetRecommendationsSafeAsync(string userId)
        {
            try
            {
                var recommendations = await GetJsonAsync<List<ProductData>>("product-service", $"/api/products/recommendations/{userId}");
                return recommendations ?? new List<ProductData>();
            }
            catch (Exception ex)
            {
                // در صورت خطا، فقط لاگ می‌نویسیم و یک لیست خالی برمی‌گردانیم تا کل فرآیند متوقف نشود.
                _logger.LogWarning(ex, "Failed to retrieve recommendations for user {UserId}. Returning empty list.", userId);
                return new List<ProductData>();
            }
        }

        // این متد بدون تغییر باقی می‌ماند، اما می‌توان منطق کشینگ مشابهی برای آن نیز پیاده‌سازی کرد.
        public async Task<OrderDetailsData?> GetOrderDetailsAsync(int orderId)
        {
            // برای سادگی، منطق کشینگ برای این متد پیاده‌سازی نشده است اما الگو مشابه GetDashboardDataAsync خواهد بود.
            try
            {
                var orderTask = GetJsonAsync<OrderData>("order-service", $"/api/orders/{orderId}");
                var shippingTask = GetJsonAsync<ShippingData>("shipping-service", $"/api/shipping/order/{orderId}");

                await Task.WhenAll(orderTask, shippingTask);

                var order = await orderTask;
                if (order == null)
                {
                    _logger.LogWarning("Could not retrieve essential order data for order {OrderId}. Aborting aggregation.", orderId);
                    return null;
                }

                var shipping = await shippingTask;

                var productTasks = order.Items.Select(item =>
                    GetJsonAsync<ProductData>("product-service", $"/api/products/{item.ProductId}")).ToArray();

                var products = await Task.WhenAll(productTasks);

                return new OrderDetailsData
                {
                    Order = order,
                    Shipping = shipping,
                    Products = products.Where(p => p != null).ToList()!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aggregating order details for order: {OrderId}", orderId);
                return null;
            }
        }

        // این متد کمکی، قلب تپنده ارتباطات است و بدون تغییر باقی می‌ماند.
        private async Task<T?> GetJsonAsync<T>(string serviceName, string requestUri) where T : class
        {
            try
            {
                return await _serviceMeshClient.GetFromJsonAsync<T>(serviceName, requestUri);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Network or HTTP error while requesting {ServiceName} at {RequestUri}.", serviceName, requestUri);
                // null برگردانده می‌شود تا متد فراخوان‌کننده تصمیم بگیرد چه کاری انجام دهد.
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "JSON parsing error for response from {ServiceName} at {RequestUri}.", serviceName, requestUri);
                return null;
            }
            // خطاهای دیگر باید به بالا پرتاب شوند تا در try-catch اصلی مدیریت شوند.
            // catch (Exception ex) { ... }
        }
    }
}