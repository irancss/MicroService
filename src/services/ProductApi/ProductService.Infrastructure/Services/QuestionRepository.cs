using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ProductDbContext _context;

        public QuestionRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Question> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByProductIdAsync(string productId)
        {
            return await _context.Questions
                                 .Where(q => q.ProductId == productId)
                                 .ToListAsync();
        }

        public async Task<Question> AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var questionToDelete = await _context.Questions.FindAsync(id);
            if (questionToDelete != null)
            {
                _context.Questions.Remove(questionToDelete);
                await _context.SaveChangesAsync();
            }
        }
    }
}
