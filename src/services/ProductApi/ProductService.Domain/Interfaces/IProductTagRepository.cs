using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductTagRepository
{
    Task<ProductTag> GetAsync(string productId, string tagId);
    Task<IEnumerable<ProductTag>> GetByProductIdAsync(string productId);
    Task<IEnumerable<ProductTag>> GetByTagIdAsync(string tagId);
    Task AddAsync(ProductTag productTag);
    Task UpdateAsync(ProductTag productTag); // If ProductTag has properties beyond keys
    Task DeleteAsync(string productId, string tagId);
}