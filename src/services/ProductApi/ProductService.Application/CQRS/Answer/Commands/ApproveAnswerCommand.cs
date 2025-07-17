using MediatR;

namespace ProductService.Application.CQRS.Answer.Commands
{
    public class ApproveAnswerCommand : IRequest
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
    }

    public class ApproveAnswerCommandHandler : IRequestHandler<ApproveAnswerCommand>
    {
        public Task<Unit> Handle(ApproveAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate approving the answer
                // In a real application, this would involve updating a database or an external service
                Console.WriteLine(
                    $"Answer with ID: {request.AnswerId} for Question ID: {request.QuestionId} has been approved.");
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
