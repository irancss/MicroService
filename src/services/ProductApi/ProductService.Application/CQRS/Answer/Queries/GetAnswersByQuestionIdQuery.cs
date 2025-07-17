using MediatR;
using Sieve.Models;

namespace ProductService.Application.CQRS.Answer.Queries
{
    public class GetAnswersByQuestionIdQuery : IRequest<string>
    {
        public string QuestionId { get; set; }
        public SieveModel SieveModel { get; set; }
    }

    public class GetAnswersByQuestionIdQueryHandler : IRequestHandler<GetAnswersByQuestionIdQuery, string>
    {
        public Task<string> Handle(GetAnswersByQuestionIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                return null;
            }
            catch
            {
                throw new Exception("");
            }
        }
    }
}
