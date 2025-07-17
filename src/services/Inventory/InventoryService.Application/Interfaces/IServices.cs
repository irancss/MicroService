using InventoryService.Domain.Events;

namespace InventoryService.Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    Task RemoveAsync(string key);
    Task RemovePatternAsync(string pattern);
}

public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent) where T : DomainEvent;
    Task PublishAsync<T>(List<T> domainEvents) where T : DomainEvent;
}

public interface IStockAlertService
{
    Task CheckAndPublishAlertsAsync(string productId);
    Task CheckMultipleProductsAsync(List<string> productIds);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
