using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface IProductImage
{
    Task<ProductImage> GetByIdAsync(int id);
    Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(string productId);
    Task<ProductImage> AddAsync(ProductImage productImage);
    Task UpdateAsync(ProductImage productImage);
    Task DeleteAsync(int id);
}