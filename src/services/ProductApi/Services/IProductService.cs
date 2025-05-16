using ProductApi.Models.Dtos;

namespace ProductApi.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(int page, int pageSize, string? category = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<ProductDto?> GetProductByIdAsync(string id);
        Task<ProductDto?> GetProductBySkuAsync(string sku);
        Task<ProductDto> CreateProductAsync(CreateProductRequest productRequest);
        Task<bool> UpdateProductAsync(string id, UpdateProductRequest productRequest);
        Task<bool> DeleteProductAsync(string id);
        // متدهای مربوط به Media و Discount در سرویس‌های جدا یا همینجا پیاده‌سازی می‌شوند
    }
    // اینترفیس برای PagedResult
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
