using ProductApi.Models.Entities;

namespace ProductApi.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string? lastId, int pageSize, string? category = null); // (int page, int pageSize, string? category = null, decimal? minPrice = null, decimal? maxPrice = null /* سایر فیلترها */);
        Task<Product?> GetByIdAsync(string id);
        Task<Product?> GetBySkuAsync(string sku);
        Task CreateAsync(Product product);
        Task<bool> UpdateAsync(string id, Product product);
        Task<bool> DeleteAsync(string id); // می‌تواند Soft Delete باشد
        Task<long> GetTotalCountAsync(string? category = null /* سایر فیلترها */);
        Task<bool> AddMediaAsync(string productId, MediaInfo mediaInfo);
        Task<bool> RemoveMediaAsync(string productId, string mediaId);
        Task UpdateRatingAsync(string productId, double newAverageRating, int newReviewCount);
    }
}
