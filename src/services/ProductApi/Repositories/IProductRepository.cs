using ProductApi.Models.Entities;

namespace ProductApi.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Gets a paginated list of products, optionally filtered by category.
        /// </summary>
        /// <param name="lastId">The last product Id from the previous page (for cursor-based pagination).</param>
        /// <param name="pageSize">Number of products to return.</param>
        /// <param name="category">Optional category filter.</param>
        /// <returns>List of products.</returns>
        Task<IEnumerable<Product>> GetAllAsync(string? lastId, int pageSize, string? category = null);

     
        /// <summary>
        /// Gets a product by its unique Id.
        /// </summary>
        /// <param name="id">Product Id.</param>
        /// <returns>The product if found, otherwise null.</returns>
        Task<Product?> GetByIdAsync(string id);

        /// <summary>
        /// Gets a product by its SKU.
        /// </summary>
        /// <param name="sku">Product SKU.</param>
        /// <returns>The product if found, otherwise null.</returns>
        Task<Product?> GetBySkuAsync(string sku);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">Product to create.</param>
        Task CreateAsync(Product product);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">Product Id.</param>
        /// <param name="product">Updated product data.</param>
        /// <returns>True if update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(string id, Product product);

        /// <summary>
        /// Deletes a product (can be soft delete).
        /// </summary>
        /// <param name="id">Product Id.</param>
        /// <returns>True if delete was successful, otherwise false.</returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Gets the total count of products, optionally filtered by category.
        /// </summary>
        /// <param name="category">Optional category filter.</param>
        /// <returns>Total count of products.</returns>
        Task<long> GetTotalCountAsync(string? category = null);

        /// <summary>
        /// Adds a media item to a product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <param name="mediaInfo">Media information.</param>
        /// <returns>True if media was added, otherwise false.</returns>
        Task<bool> AddMediaAsync(string productId, MediaInfo mediaInfo);

        /// <summary>
        /// Removes a media item from a product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <param name="mediaId">Media Id.</param>
        /// <returns>True if media was removed, otherwise false.</returns>
        Task<bool> RemoveMediaAsync(string productId, string mediaId);

        /// <summary>
        /// Updates the average rating and review count for a product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <param name="newAverageRating">New average rating.</param>
        /// <param name="newReviewCount">New review count.</param>
        Task UpdateRatingAsync(string productId, double newAverageRating, int newReviewCount);

        /// <summary>
        /// Searches products by name or description.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="pageSize">Number of products to return.</param>
        /// <param name="lastId">The last product Id from the previous page (for cursor-based pagination).</param>
        /// <returns>List of products matching the search.</returns>
        Task<IEnumerable<Product>> SearchAsync(string query, int pageSize, string? lastId = null);

        /// <summary>
        /// Gets products by a list of Ids.
        /// </summary>
        /// <param name="ids">List of product Ids.</param>
        /// <returns>List of products.</returns>
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<string> ids);

        /// <summary>
        /// Updates the IsActive status of a product.
        /// </summary>
        /// <param name="id">Product Id.</param>
        /// <param name="isActive">New active status.</param>
        /// <returns>True if update was successful, otherwise false.</returns>
        Task<bool> UpdateIsActiveAsync(string id, bool isActive);


    }
}
