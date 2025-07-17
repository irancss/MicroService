using ProductService.Domain.Models;

// Assuming Product entity is defined in a namespace like ProductService.Domain.Entities
// using ProductService.Domain.Entities; 
// Or if Product is in the same root domain namespace:
// using ProductService.Domain; 

namespace ProductService.Domain.Interfaces
{
    // If Product is not in the same namespace or a parent one, ensure you have a using statement for it.
    // For example: using ProductService.Domain.Entities;
    // For this example, we assume 'Product' is an accessible type.

    public interface IProductRepository
    {
        /// <summary>
        /// Gets a product by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the product.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The product with the specified identifier, or null if not found.</returns>
        Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all products, with optional pagination.
        /// </summary>
        /// <param name="skip">Optional. The number of products to skip (for pagination).</param>
        /// <param name="take">Optional. The number of products to take (for pagination).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of products.</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync(int? skip = null, int? take = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new product.
        /// </summary>
        /// <param name="product">The product to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task AddProductAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple products in a batch.
        /// </summary>
        /// <param name="products">The collection of products to add.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task AddProductsAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a product by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the product to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task DeleteProductAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes multiple products by their identifiers in a batch.
        /// </summary>
        /// <param name="ids">The collection of identifiers of the products to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task DeleteProductsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a product exists by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the product.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        Task<bool> ProductExistsAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a product exists by its name.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if a product with the specified name exists, otherwise false.</returns>
        Task<bool> ProductExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets products by category, with optional pagination.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="skip">Optional. The number of products to skip (for pagination).</param>
        /// <param name="take">Optional. The number of products to take (for pagination).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of products in the specified category.</returns>
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int? skip = null, int? take = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches products by name, with optional pagination.
        /// </summary>
        /// <param name="name">The name or part of the name to search for.</param>
        /// <param name="skip">Optional. The number of products to skip (for pagination).</param>
        /// <param name="take">Optional. The number of products to take (for pagination).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A list of products matching the search criteria.</returns>
        Task<IEnumerable<Product>> SearchProductsByNameAsync(string name, int? skip = null, int? take = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of products.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The total number of products.</returns>
        Task<int> GetProductCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of products for a specific category.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The total number of products in the specified category.</returns>
        Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    }
}
