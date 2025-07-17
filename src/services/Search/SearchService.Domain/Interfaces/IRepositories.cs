using SearchService.Domain.Entities;

namespace SearchService.Domain.Interfaces;

/// <summary>
/// Repository interface for Elasticsearch operations
/// </summary>
public interface ISearchRepository
{
    /// <summary>
    /// Index a product document
    /// </summary>
    Task<bool> IndexProductAsync(ProductDocument product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a product document
    /// </summary>
    Task<bool> UpdateProductAsync(string productId, ProductDocument product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a product from the index
    /// </summary>
    Task<bool> DeleteProductAsync(string productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk index multiple products
    /// </summary>
    Task<bool> BulkIndexProductsAsync(IEnumerable<ProductDocument> products, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if Elasticsearch is available
    /// </summary>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<ProductDocument?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service interface for user personalization data
/// </summary>
public interface IUserPersonalizationService
{
    /// <summary>
    /// Get user personalization data for search boosting
    /// </summary>
    Task<UserPersonalizationData?> GetUserPersonalizationAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user personalization data based on interactions
    /// </summary>
    Task UpdateUserPersonalizationAsync(string userId, UserPersonalizationData data, CancellationToken cancellationToken = default);
}
