using MediatR;

namespace ProductService.Application.CQRS.Answer.Queries
{
    public class GetAnswerByIdQuery : IRequest<string>
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
    }

    public class GetAnswerByIdQueryHandler : IRequestHandler<GetAnswerByIdQuery, string>
    {
        public Task<string> Handle(GetAnswerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate fetching the answer by ID
                // In a real application, this would involve querying a database or an external service
                return Task.FromResult(
                    $"Answer for Question ID: {request.QuestionId} and Answer ID: {request.AnswerId}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
