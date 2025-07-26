using MediatR;
using ProductService.Application.DTOs.Brand;

namespace ProductService.Application.CQRS.Brand.Commands
{
    public class UpdateBrandCommand : IRequest<Unit>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand , Unit>
    {
        public UpdateBrandCommandHandler()
        {
            
        }

        public async Task<Unit> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            // Simulate updating the brand
            // In a real application, this would involve updating a database or an external service
            Console.WriteLine($"Brand with ID: {request.Id} has been updated with Name: {request.Name}, Slug: {request.Slug}, Description: {request.Description}, LogoUrl: {request.LogoUrl}, IsActive: {request.IsActive}, DisplayOrder: {request.DisplayOrder}");
            return Unit.Value;
        }
    }

}
