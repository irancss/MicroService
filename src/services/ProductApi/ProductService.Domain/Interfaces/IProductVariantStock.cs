using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductVariantStock
{
    Task<ProductVariantStock> GetByIdAsync(int id);
    Task<ProductVariantStock> GetByProductVariantIdAsync(string productVariantId);
    Task<IEnumerable<ProductVariantStock>> GetStockByProductIdAsync(Guid productId); // For all variants of a product
    Task<ProductVariantStock> AddAsync(ProductVariantStock stock);
    Task UpdateAsync(ProductVariantStock stock);
    Task DeleteAsync(int id);
}