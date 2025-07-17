using MediatR;

namespace ProductService.Application.CQRS.Answer.Commands
{
    public class UpdateAnswerCommand : IRequest
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
    public class UpdateAnswerCommandHandler : IRequestHandler<UpdateAnswerCommand>
    {
        public Task<Unit> Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Simulate updating the answer
                // In a real application, this would involve updating a database or an external service
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
