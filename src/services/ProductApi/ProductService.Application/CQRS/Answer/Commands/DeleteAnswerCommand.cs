using MediatR;

namespace ProductService.Application.CQRS.Answer.Commands
{
    public class DeleteAnswerCommand : IRequest
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
    }
    public class DeleteAnswerCommandHandler : IRequestHandler<DeleteAnswerCommand>
    {
        public Task<Unit> Handle(DeleteAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate deleting the answer
                // In a real application, this would involve deleting from a database or an external service
                return Task.FromResult(Unit.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
