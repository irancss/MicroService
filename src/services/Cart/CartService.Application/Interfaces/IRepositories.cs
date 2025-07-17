using Cart.Domain.Entities;
using Cart.Domain.ValueObjects;

namespace Cart.Application.Interfaces;

public interface ICartRepository
{
    Task<ShoppingCart?> GetByUserIdAsync(string userId);
    Task<ShoppingCart?> GetByGuestIdAsync(string guestId);
    Task<ShoppingCart?> GetByIdAsync(string cartId);
    Task SaveAsync(ShoppingCart cart);
    Task DeleteAsync(string cartId);
    Task<bool> ExistsAsync(string cartId);
    Task<List<ShoppingCart>> GetAbandonedCartsAsync(DateTime abandonmentThreshold);
    Task<List<ShoppingCart>> GetExpiredCartsAsync(DateTime expirationThreshold);
}

public interface ICartConfigurationService
{
    Task<CartConfiguration> GetConfigurationAsync();
    Task UpdateConfigurationAsync(CartConfiguration configuration);
}

public interface IInventoryGrpcClient
{
    Task<bool> CheckStockAvailabilityAsync(string productId, int quantity);
    Task<decimal?> GetCurrentPriceAsync(string productId);
    Task<Dictionary<string, bool>> CheckMultipleStockAvailabilityAsync(Dictionary<string, int> productQuantities);
    Task<Dictionary<string, decimal>> GetMultiplePricesAsync(List<string> productIds);
}

public interface ICatalogGrpcClient
{
    Task<ProductInfo?> GetProductInfoAsync(string productId);
    Task<List<ProductInfo>> GetMultipleProductInfoAsync(List<string> productIds);
}

public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent) where T : class;
    Task PublishMultipleAsync<T>(List<T> domainEvents) where T : class;
}

public interface INotificationService
{
    Task SendCartAbandonmentEmailAsync(string userId, ShoppingCart cart, int notificationNumber);
    Task SendCartAbandonmentSmsAsync(string userId, ShoppingCart cart, int notificationNumber);
    Task SendNextPurchaseActivatedNotificationAsync(string userId, int itemsCount);
}

public class ProductInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? CategoryId { get; set; }
    public string? BrandId { get; set; }
}
