using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductSpecification
{
    Task<ProductSpecification> GetByIdAsync(int id);
    Task<IEnumerable<ProductSpecification>> GetSpecificationsByProductIdAsync(string productId);
    Task<ProductSpecification> AddAsync(ProductSpecification specification);
    Task UpdateAsync(ProductSpecification specification);
    Task DeleteAsync(int id);
}