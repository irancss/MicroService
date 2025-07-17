using ProductService.Application.DTOs.Answer;

namespace ProductService.Application.Interfaces
{
    public interface IAnswerService
    {

        Task<AnswerDto?> GetAnswerByIdAsync(string answerId);

        Task<IEnumerable<AnswerDto>> GetAnswersByQuestionIdAsync(string questionId);

        Task<AnswerDto> CreateAnswerAsync(CreateAnswerDto createAnswerDto);

        Task<bool> UpdateAnswerAsync(string answerId, UpdateAnswerDto updateAnswerDto);

        Task<bool> DeleteAnswerAsync(string answerId);
    }
}
