using ProductApi.Models.Entities;

namespace ProductApi.Repositories
{
    public interface IReviewRepository
    {
        /// <summary>
        /// Gets all reviews for a specific product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <param name="lastId">The last review Id from the previous page (for cursor-based pagination).</param>
        /// <param name="pageSize">Number of reviews to return.</param>
        /// <returns>List of reviews.</returns>
        Task<IEnumerable<Review>> GetByProductIdAsync(string productId, string? lastId, int pageSize);

        /// <summary>
        /// Gets a review by its unique Id.
        /// </summary>
        /// <param name="id">Review Id.</param>
        /// <returns>The review if found, otherwise null.</returns>
        Task<Review?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new review.
        /// </summary>
        /// <param name="review">Review to create.</param>
        Task CreateAsync(Review review);

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="id">Review Id.</param>
        /// <param name="review">Updated review data.</param>
        /// <returns>True if update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(string id, Review review);

        /// <summary>
        /// Deletes a review.
        /// </summary>
        /// <param name="id">Review Id.</param>
        /// <returns>True if delete was successful, otherwise false.</returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Gets the total count of reviews for a product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Total count of reviews.</returns>
        Task<long> GetTotalCountByProductAsync(string productId);
    }
}
