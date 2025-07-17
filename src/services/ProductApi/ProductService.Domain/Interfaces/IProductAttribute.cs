using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductAttribute
{
    Task<ProductAttribute> GetByIdAsync(int id);
    Task<IEnumerable<ProductAttribute>> GetAttributesByProductIdAsync(string productId);
    Task<ProductAttribute> AddAsync(ProductAttribute attribute);
    Task UpdateAsync(ProductAttribute attribute);
    Task DeleteAsync(int id);
}