using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;

// Assuming Answer entity looks something like this:
// namespace ProductService.Domain.Entities // Or wherever your entities are
// {
//     public class Answer
//     {
//         public int Id { get; set; }
//         public string Text { get; set; } // Example property
//         public int QuestionId { get; set; } // Foreign key to Question
//         // Add other relevant properties
//     }
// }


namespace ProductService.Infrastructure.Services
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ProductDbContext _context;

        public AnswerRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Answer> GetByIdAsync(string id)
        {
            return await _context.Answers.FindAsync(id);
        }

        public async Task<IEnumerable<Answer>> GetByQuestionIdAsync(string questionId)
        {
            return await _context.Answers
                                 .Where(a => a.QuestionId == questionId)
                                 .ToListAsync();
        }

        public async Task<Answer> AddAsync(Answer answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException(nameof(answer));
            }

            await _context.Answers.AddAsync(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task UpdateAsync(Answer answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException(nameof(answer));
            }

            _context.Answers.Update(answer);
            // Or if you prefer to be more explicit about the state:
            // _context.Entry(answer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var answerToDelete = await _context.Answers.FindAsync(id);
            if (answerToDelete != null)
            {
                _context.Answers.Remove(answerToDelete);
                await _context.SaveChangesAsync();
            }
            // Optionally, throw an exception if not found, or handle it as per requirements.
        }
    }
}
