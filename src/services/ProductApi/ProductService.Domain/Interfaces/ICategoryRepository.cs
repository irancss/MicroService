using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category> GetByIdAsync(string id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(string id);
}