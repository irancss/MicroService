using ProductService.Application.DTOs.Product;
using ProductService.Domain.Interfaces;
using AutoMapper;
using BuildingBlocks.Application.CQRS.Queries;

namespace ProductService.Application.CQRS.Queries
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            // از متد جدیدی در ریپازیتوری استفاده می‌کنیم که Include ها را انجام دهد
            var product = await _productRepository.GetByIdWithDetailsAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }
    }
}
