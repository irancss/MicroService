using System.Text.Json;
using BuildingBlocks.Messaging;
using Cart.Application.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Cart.Infrastructure.Services;

namespace Cart.Infrastructure.BackgroundJobs
{
    public class CartJobs
    {
        private readonly ILogger<CartJobs> _logger;

        public CartJobs(ILogger<CartJobs> logger)
        {
            _logger = logger;
        }

        // [مهم] تمام وابستگی‌های مورد نیاز برای یک بار اجرا را به عنوان پارامتر متد دریافت می‌کنیم
        // Hangfire به طور خودکار این سرویس‌ها را از DI Container تزریق می‌کند.
        public async Task ProcessExpiredCartsAsync(
            ICartConfigurationService configService,
            IConnectionMultiplexer redis,
            IMessageBus messageBus)
        {
            _logger.LogInformation("Hangfire Job: Processing expired carts started at {Time}", DateTime.UtcNow);

            try
            {
                var config = await configService.GetConfigurationAsync();
                var database = redis.GetDatabase();
                var server = redis.GetServer(redis.GetEndPoints().First());

                var keys = server.Keys(pattern: "cart:active:*");
                int processedCount = 0;

                foreach (var key in keys)
                {
                    var cartJson = await database.StringGetAsync(key);
                    if (cartJson.IsNullOrEmpty) continue;

                    // استفاده از System.Text.Json برای هماهنگی
                    var cart = JsonSerializer.Deserialize<Domain.Entities.ActiveCart>(cartJson!);
                    if (cart is null) continue;

                    if ((DateTime.UtcNow - cart.LastModifiedUtc).TotalMinutes > config.ActiveCartExpiryMinutes)
                    {
                        _logger.LogWarning("Cart {CartId} for user {UserId} has expired. Releasing stock.", cart.Id, cart.UserId);

                        var releaseCommand = new ReleaseStockCommand(cart.Id, "Cart expired");
                        await messageBus.SendAsync(releaseCommand); // CancellationToken اینجا لازم نیست
                        await database.KeyDeleteAsync(key);
                        processedCount++;
                    }
                }

                _logger.LogInformation("Hangfire Job: Finished processing. Found and processed {Count} expired carts.", processedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the ProcessExpiredCartsAsync Hangfire job.");
                // Hangfire به صورت خودکار این Job را دوباره تلاش خواهد کرد.
                throw; // مهم است که خطا را دوباره پرتاب کنید تا Hangfire متوجه شکست Job شود.
            }
        }
    }
}
