using ProductApi.Models.Entities;

namespace ProductApi.Repositories
{
    public interface IQuestionRepository
    {
        /// <summary>
        /// Gets all questions for a specific product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <param name="lastId">The last question Id from the previous page (for cursor-based pagination).</param>
        /// <param name="pageSize">Number of questions to return.</param>
        /// <returns>List of questions.</returns>
        Task<IEnumerable<Question>> GetByProductIdAsync(string productId, string? lastId, int pageSize);

        /// <summary>
        /// Gets a question by its unique Id.
        /// </summary>
        /// <param name="id">Question Id.</param>
        /// <returns>The question if found, otherwise null.</returns>
        Task<Question?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="question">Question to create.</param>
        Task CreateAsync(Question question);

        /// <summary>
        /// Updates an existing question.
        /// </summary>
        /// <param name="id">Question Id.</param>
        /// <param name="question">Updated question data.</param>
        /// <returns>True if update was successful, otherwise false.</returns>
        Task<bool> UpdateAsync(string id, Question question);

        /// <summary>
        /// Deletes a question.
        /// </summary>
        /// <param name="id">Question Id.</param>
        /// <returns>True if delete was successful, otherwise false.</returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Gets the total count of questions for a product.
        /// </summary>
        /// <param name="productId">Product Id.</param>
        /// <returns>Total count of questions.</returns>
        Task<long> GetTotalCountByProductAsync(string productId);
    }

}
