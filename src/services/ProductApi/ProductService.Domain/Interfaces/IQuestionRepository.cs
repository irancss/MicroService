
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByProductIdAsync(string productId);
        Task<Question> AddAsync(Question question); // Changed to return Task<Question>
        Task UpdateAsync(Question question);
        Task DeleteAsync(int id);
        // Consider adding batch operations if needed
        // Task AddRangeAsync(IEnumerable<Question> questions);
        // Task UpdateRangeAsync(IEnumerable<Question> questions);
    }
}