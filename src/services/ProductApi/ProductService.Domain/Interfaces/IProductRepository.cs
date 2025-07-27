using BuildingBlocks.Abstractions;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;

// Assuming Product entity is defined in a namespace like ProductService.Domain.Entities
// using ProductService.Domain.Entities; 
// Or if Product is in the same root domain namespace:
// using ProductService.Domain; 

namespace ProductService.Domain.Interfaces
{
    // If Product is not in the same namespace or a parent one, ensure you have a using statement for it.
    // For example: using ProductService.Domain.Entities;
    // For this example, we assume 'Product' is an accessible type.

    public interface IProductRepository : IRepositoryAsync<Product>
    {
        Task<Product?> GetByIdWithDetailsAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> IsSkuUniqueAsync(Sku sku, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(Guid productId); // override برای استفاده از Guid
        // متدهای دیگر برای جستجو و فیلتر پیشرفته اینجا اضافه می‌شوند
    }
}
