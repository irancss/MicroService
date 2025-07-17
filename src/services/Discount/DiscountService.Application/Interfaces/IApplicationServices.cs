namespace DiscountService.Application.Interfaces;

/// <summary>
/// Interface for caching service operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get value from cache
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;
    
    /// <summary>
    /// Set value in cache with expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
    
    /// <summary>
    /// Remove value from cache
    /// </summary>
    Task RemoveAsync(string key);
    
    /// <summary>
    /// Remove values matching pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern);
    
    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);
}

/// <summary>
/// Interface for application database context
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Save changes to database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for current user service
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Get current user ID
    /// </summary>
    Guid? UserId { get; }
    
    /// <summary>
    /// Get current user email
    /// </summary>
    string? UserEmail { get; }
    
    /// <summary>
    /// Check if current user is admin
    /// </summary>
    bool IsAdmin { get; }
}
