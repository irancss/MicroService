namespace InventoryService.Domain.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

public interface IStockAlertService
{
    Task CheckAndPublishAlertsAsync(string productId, int currentStock, int? lowThreshold, int? excessThreshold, CancellationToken cancellationToken = default);
}
