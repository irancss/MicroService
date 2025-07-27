using Ardalis.GuardClauses;
using ProductService.Domain.Interfaces;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Brand
{
    public class DeleteBrandCommandHandler : ICommandHandler<DeleteBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;

        public DeleteBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);

            Guard.Against.NotFound(request.BrandId, brand, nameof(brand));

            await _brandRepository.DeleteAsync(brand);
        }
    }
}
