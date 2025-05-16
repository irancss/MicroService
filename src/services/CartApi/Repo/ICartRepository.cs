using System.Text.Json;
using CartApi.Models;
using StackExchange.Redis;

namespace CartApi.Repo
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string userId);
        Task<Cart> UpdateCartAsync(Cart cart); // هم برای ایجاد و هم بروزرسانی
        Task<bool> DeleteCartAsync(string userId);
    }

    public class RedisCartRepository : ICartRepository
    {
        private readonly IDatabase _redisDatabase;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCartRepository(IConnectionMultiplexer redisConnection)
        {
            _redisDatabase = redisConnection.GetDatabase();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            var serializedCart = await _redisDatabase.StringGetAsync($"cart:{userId}");
            if (serializedCart.IsNullOrEmpty)
            {
                return null; // یا یک سبد خالی جدید برگردانید: return new Cart(userId);
            }

            try
            {
                var cart = JsonSerializer.Deserialize<Cart>(serializedCart.ToString());
                if (cart != null)
                {
                    // UserId در JSON ذخیره نمی‌شود، پس باید اینجا تنظیم شود
                    // این کار با استفاده از یک private setter و سازنده مناسب انجام می‌شود
                    // یا می‌توان یک فیلد خصوصی در Cart تعریف کرد و با Reflection مقداردهی کرد
                    // راه ساده‌تر: UserId را در سازنده Cart قرار دهیم
                    var cartWithUserId = new Cart(userId)
                    {
                        Items = cart.Items,
                        DiscountAmount = cart.DiscountAmount,
                        AppliedDiscountCode = cart.AppliedDiscountCode
                        // سایر پراپرتی‌ها...
                    };
                    return cartWithUserId;
                }
            }
            catch (JsonException ex)
            {
                // Log the error
                Console.WriteLine($"Error deserializing cart for user {userId}: {ex.Message}");
                // شاید بهتر باشد سبد نامعتبر را حذف کنیم
                await DeleteCartAsync(userId);
                return null;
            }
            return null;
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            var serializedCart = JsonSerializer.Serialize(cart);
            // زمان انقضا (TTL) برای سبدهای مهمان یا حتی سبدهای عادی خوب است
            // TimeSpan.FromDays(7) یا TimeSpan.FromHours(2) برای مهمان
            var success = await _redisDatabase.StringSetAsync($"cart:{cart.UserId}", serializedCart, TimeSpan.FromDays(7));

            if (!success)
            {
                // خطا در ذخیره‌سازی - لاگ یا مدیریت خطا
                throw new Exception($"Failed to update cart for user {cart.UserId}");
            }
            return await GetCartAsync(cart.UserId) ?? cart; // بازگرداندن سبد ذخیره شده (برای اطمینان)

        }

        public async Task<bool> DeleteCartAsync(string userId)
        {
            return await _redisDatabase.KeyDeleteAsync($"cart:{userId}");
        }
    }
}
