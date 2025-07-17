using ProductService.Application.DTOs.Answer;
using ProductService.Application.Interfaces;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;

        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<AnswerDto?> GetAnswerByIdAsync(string answerId)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null) return null;

            return new AnswerDto
            {
                Id = answer.Id,
                QuestionId = answer.QuestionId,
                AnswerText = answer.AnswerText,
                CreatedAt = answer.CreatedAt
            };
        }

        public async Task<IEnumerable<AnswerDto>> GetAnswersByQuestionIdAsync(string questionId)
        {
            var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
            return answers.Select(a => new AnswerDto
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                AnswerText = a.AnswerText,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto createAnswerDto)
        {
            var answer = new Answer(
                createAnswerDto.QuestionId,
                createAnswerDto.UserId,
                createAnswerDto.AnswerText,
                createAnswerDto.IsAdminAnswer
            );

            var created = await _answerRepository.AddAsync(answer);

            return new AnswerDto
            {
                Id = created.Id,
                QuestionId = created.QuestionId,
                AnswerText = created.AnswerText,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<bool> UpdateAnswerAsync(string answerId, UpdateAnswerDto updateAnswerDto)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null) return false;

            answer.AnswerText = updateAnswerDto.AnswerText;
            await _answerRepository.UpdateAsync(answer);
            return true;
        }

        public async Task<bool> DeleteAnswerAsync(string answerId)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null) return false;

            await _answerRepository.DeleteAsync(answerId);
            return true;
        }
       
    }
}
