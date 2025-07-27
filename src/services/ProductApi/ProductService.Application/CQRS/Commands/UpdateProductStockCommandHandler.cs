using ProductService.Domain.Interfaces;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Commands
{
    public class UpdateProductStockCommandHandler : ICommandHandler<UpdateProductStockCommand>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductStockCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);

            // استفاده از GuardClause برای بررسی null بودن
            Guard.Against.NotFound(request.ProductId, product, nameof(product));

            product.UpdateStock(request.NewQuantity);

            // نیازی به فراخوانی UpdateAsync نیست چون EF Core تغییرات را Track می‌کند.
            // SaveChanges توسط TransactionBehavior انجام می‌شود.
        }
    }
}
