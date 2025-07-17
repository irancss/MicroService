using MediatR;

namespace ProductService.Application.CQRS.Brand.Commands
{
    public class DeleteBrandCommand : IRequest<string>
    {
        public string Id { get; set; }
    }
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, string>
    {
        public Task<string> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            // Simulate deleting the brand
            // In a real application, this would involve deleting from a database or an external service
            Console.WriteLine($"Brand with ID: {request.Id} has been deleted.");
            return Task.FromResult(request.Id);
        }
    }
}
