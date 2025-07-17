using MediatR;

namespace ProductService.Application.CQRS.Brand.Commands
{
    public class CreateBrandCommand : IRequest<string>
    {
        public CreateBrandCommand()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; private set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, string>
    {
        // This handler would typically interact with the database to create a new brand
        public Task<string> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            // Logic to create the brand in the database
            // For now, we will just return the Id of the created brand
            return Task.FromResult(request.Id);
        }
    }
}
