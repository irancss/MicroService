using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductTagRepository
{
    Task<ProductTag> GetAsync(Guid productId, Guid tagId);
    Task<IEnumerable<ProductTag>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<ProductTag>> GetByTagIdAsync(Guid tagId);
    Task AddAsync(ProductTag productTag);
    Task UpdateAsync(ProductTag productTag); // If ProductTag has properties beyond keys
    Task DeleteAsync(Guid productId, Guid tagId);
}