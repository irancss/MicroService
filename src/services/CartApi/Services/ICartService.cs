using CartApi.Models;
using CartApi.Repo;

namespace CartApi.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(string userId);
        Task<Cart> AddItemAsync(string userId, AddItemRequest itemRequest);
        Task<Cart> UpdateItemQuantityAsync(string userId, string productId, int quantity);
        Task<Cart> RemoveItemAsync(string userId, string productId);
        Task<bool> ClearCartAsync(string userId);
        Task<Cart> ApplyDiscountAsync(string userId, string discountCode);
        // (اختیاری) Task HandleOrderConfirmationAsync(string userId); // برای پاک کردن سبد پس از سفارش

    }
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<CartService> _logger;
        // private readonly IProductServiceClient _productServiceClient; // تزریق کلاینت سرویس محصول
        // private readonly IDiscountServiceClient _discountServiceClient; // تزریق کلاینت سرویس تخفیف

        public CartService(ICartRepository cartRepository, ILogger<CartService> logger /*, IProductServiceClient productServiceClient, IDiscountServiceClient discountServiceClient */)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            // _productServiceClient = productServiceClient;
            // _discountServiceClient = discountServiceClient;
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            _logger.LogInformation("Getting cart for user {UserId}", userId);
            return await _cartRepository.GetCartAsync(userId);
        }

        public async Task<Cart> AddItemAsync(string userId, AddItemRequest itemRequest)
        {
            _logger.LogInformation("Adding item {ProductId} (Qty: {Quantity}) for user {UserId}", itemRequest.ProductId, itemRequest.Quantity, userId);
            var cart = await _cartRepository.GetCartAsync(userId) ?? new Cart(userId);

            // --- دریافت اطلاعات محصول از سرویس دیگر ---
            // ProductInfo productInfo = await _productServiceClient.GetProductByIdAsync(itemRequest.ProductId);
            // if (productInfo == null) throw new KeyNotFoundException($"Product {itemRequest.ProductId} not found.");
            // // در اینجا می‌توان موجودی را هم چک کرد، هرچند چک نهایی معمولا در سرویس سفارش انجام می‌شود

            // برای سادگی، فعلا اطلاعات محصول را ثابت در نظر می‌گیریم:
            var productInfo = new { Name = $"Product {itemRequest.ProductId}", Price = 10.0m }; // قیمت فرضی

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemRequest.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += itemRequest.Quantity;
                // قیمت واحد را بروز نکنید مگر اینکه سیاست کسب‌وکار این باشد
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = itemRequest.ProductId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = productInfo.Price, // قیمت از سرویس محصول
                    ProductName = productInfo.Name // نام از سرویس محصول
                    // ImageUrl = productInfo.ImageUrl
                });
            }
            // قیمت کل و نهایی در پراپرتی‌های خود Cart محاسبه می‌شود
            return await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task<Cart> UpdateItemQuantityAsync(string userId, string productId, int quantity)
        {
            _logger.LogInformation("Updating item {ProductId} to quantity {Quantity} for user {UserId}", productId, quantity, userId);
            if (quantity <= 0)
            {
                return await RemoveItemAsync(userId, productId); // اگر تعداد صفر یا منفی شد، حذف کن
            }

            var cart = await GetRequiredCartAsync(userId); // متد کمکی برای گرفتن سبد یا throw exception

            var itemToUpdate = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToUpdate == null)
            {
                throw new KeyNotFoundException($"Item {productId} not found in cart for user {userId}.");
            }

            itemToUpdate.Quantity = quantity;
            // در صورت نیاز به بررسی مجدد موجودی با سرویس محصول تماس بگیرید

            return await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task<Cart> RemoveItemAsync(string userId, string productId)
        {
            _logger.LogInformation("Removing item {ProductId} for user {UserId}", productId, userId);
            var cart = await GetRequiredCartAsync(userId);
            var itemRemoved = cart.Items.RemoveAll(i => i.ProductId == productId);

            if (itemRemoved == 0)
            {
                _logger.LogWarning("Item {ProductId} not found to remove for user {UserId}", productId, userId);
                // شاید بهتر باشد خطایی برنگردانیم و فقط سبد فعلی را بدهیم
                return cart;
            }

            // اگر آخرین آیتم حذف شد، کل سبد را حذف کنیم؟ (بستگی به نیاز دارد)
            if (!cart.Items.Any())
            {
                await _cartRepository.DeleteCartAsync(userId);
                return cart; // برگرداندن سبد خالی قبل از حذف
            }
            return await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}", userId);
            return await _cartRepository.DeleteCartAsync(userId);
        }

        public async Task<Cart> ApplyDiscountAsync(string userId, string discountCode)
        {
            _logger.LogInformation("Applying discount code {DiscountCode} for user {UserId}", discountCode, userId);
            var cart = await GetRequiredCartAsync(userId);

            // --- اعتبارسنجی و دریافت مبلغ تخفیف از سرویس تخفیف ---
            // DiscountInfo discountInfo = await _discountServiceClient.ValidateAndGetDiscount(discountCode, cart.TotalPrice, userId);
            // if (discountInfo == null || !discountInfo.IsValid) {
            //     throw new ArgumentException($"Discount code {discountCode} is invalid or not applicable.");
            // }

            // برای سادگی، تخفیف ثابت در نظر می‌گیریم:
            var discountInfo = new { IsValid = true, Amount = 5.0m }; // تخفیف 5 واحدی فرضی

            if (discountInfo.IsValid)
            {
                cart.DiscountAmount = discountInfo.Amount;
                cart.AppliedDiscountCode = discountCode;
                _logger.LogInformation("Applied discount {DiscountAmount} with code {DiscountCode} for user {UserId}", cart.DiscountAmount, cart.AppliedDiscountCode, userId);
            }
            else
            {
                // پاک کردن تخفیف قبلی اگر کد نامعتبر است
                cart.DiscountAmount = 0;
                cart.AppliedDiscountCode = null;
                _logger.LogWarning("Discount code {DiscountCode} was invalid for user {UserId}", discountCode, userId);
                // می‌توانید خطا برگردانید یا فقط تخفیف را صفر کنید
                throw new ArgumentException($"Discount code {discountCode} is invalid.");
            }


            return await _cartRepository.UpdateCartAsync(cart);
        }

        // متد کمکی برای اطمینان از وجود سبد
        private async Task<Cart> GetRequiredCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartAsync(userId);
            if (cart == null)
            {
                _logger.LogError("Cart not found for user {UserId} when it was required.", userId);
                throw new KeyNotFoundException($"Cart not found for user {userId}.");
            }
            return cart;
        }

        // این متد توسط یک Consumer کافکا یا مکانیسم دیگری فراخوانی می‌شود
        // وقتی سرویس سفارش، رویداد OrderPlaced را منتشر می‌کند.
        /*
        public async Task HandleOrderConfirmationAsync(string userId)
        {
            _logger.LogInformation("Order confirmed for user {UserId}. Clearing cart.", userId);
            await ClearCartAsync(userId);
        }
        */

        public class RedisSettings
        {
            public string ConnectionString { get; set; }
            public int TTLMinutes { get; set; }
        }
    }
}
