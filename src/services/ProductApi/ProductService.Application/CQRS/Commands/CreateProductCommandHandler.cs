using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;
using BuildingBlocks.Application.CQRS.Commands;

namespace ProductService.Application.CQRS.Commands
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // تبدیل primitive types به Value Objects
            var productName = ProductName.For(request.Name);
            var productPrice = ProductPrice.For(request.Price);
            var sku = Sku.For(request.Sku);

            // بررسی اینکه آیا محصولی با این SKU از قبل وجود دارد یا نه
            if (await _productRepository.IsSkuUniqueAsync(sku) == false)
            {
                // از یک Exception سفارشی استفاده می‌کنیم که بعدا در Middleware به 409 Conflict تبدیل شود
                throw new SkuNotUniqueException(sku);
            }

            var product = Product.Create(
                productName,
                productPrice,
                sku,
                request.Stock,
                request.Description,
                request.BrandId
            );

            request.CategoryIds?.ForEach(catId => product.AddCategory(catId));

            await _productRepository.AddAsync(product);

            // TransactionBehavior که در BuildingBlocks ساختیم، به صورت خودکار SaveChangesAsync را فراخوانی و Commit می‌کند.
            // بنابراین نیازی به فراخوانی دستی _unitOfWork.SaveChangesAsync() در اینجا نیست.

            return product.Id;
        }
    }
}
