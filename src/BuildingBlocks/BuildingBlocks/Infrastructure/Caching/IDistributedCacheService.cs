namespace BuildingBlocks.Infrastructure.Caching;

/// <summary>
/// Interface for distributed caching operations across microservices.
/// </summary>
public interface IDistributedCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) where T : class;
    Task SetStringAsync(string key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task RefreshAsync(string key, CancellationToken cancellationToken = default);
}