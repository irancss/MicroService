using ProductService.Domain.Models;

namespace ProductService.Domain.Interfaces;

public interface ITagRepository
{
    Task<Tag> GetByIdAsync(string id);
    Task<Tag> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag> AddAsync(Tag tag);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(string id);
}