using Ardalis.GuardClauses;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Commands
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            Guard.Against.NotFound(request.ProductId, product, nameof(product));

            // فراخوانی متدهای موجودیت برای اعمال تغییرات
            product.Update(
                ProductName.For(request.Name),
                request.Description,
                ProductPrice.For(request.Price), // <- قیمت جدید را اینجا پاس می‌دهیم
                request.BrandId
            );

            // ...
            // فرض می‌کنیم Command شما قیمت را هم شامل می‌شود
            // product.UpdatePrice(ProductPrice.For(request.Price));
        }
    }
}
