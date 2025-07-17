using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Domain.Interfaces
{
    public interface IAnswerRepository
    {
        Task<Answer> GetByIdAsync(string id);
        Task<IEnumerable<Answer>> GetByQuestionIdAsync(string questionId);
        Task<Answer> AddAsync(Answer answer); // Ensure it returns Task<Answer>
        Task UpdateAsync(Answer answer);
        Task DeleteAsync(string id);
        // Consider adding batch operations if needed
        // Task AddRangeAsync(IEnumerable<Answer> answers);
        // Task UpdateRangeAsync(IEnumerable<Answer> answers);
    }
}