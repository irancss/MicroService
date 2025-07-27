namespace Cart.Application.Interfaces;

public interface IInventoryClient
{
    Task<decimal?> GetCurrentPriceAsync(string productId, CancellationToken cancellationToken = default);
    Task<bool> CheckStockAvailabilityAsync(string productId, int quantity, CancellationToken cancellationToken = default);
    Task ReserveStockAsync(string cartId, Dictionary<string, int> items, CancellationToken cancellationToken = default);
    Task ReleaseStockAsync(string cartId, string reason, CancellationToken cancellationToken = default);
}