using Ardalis.GuardClauses;
using ProductService.Domain.Interfaces;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Brand
{
    public class UpdateBrandCommandHandler : ICommandHandler<UpdateBrandCommand>
    {
        private readonly IBrandRepository _brandRepository;

        public UpdateBrandCommandHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);

            Guard.Against.NotFound(request.BrandId, brand, nameof(brand));

            brand.Update(request.Name, request.Description);

            // EF Core تغییرات را track می‌کند. TransactionBehavior ذخیره را انجام می‌دهد.
        }
    }
}
