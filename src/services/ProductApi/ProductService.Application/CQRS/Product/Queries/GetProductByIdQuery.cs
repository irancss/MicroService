using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Product;

namespace ProductService.Application.CQRS.Product.Queries;
 
 public class GetProductByIdQuery : IRequest<ProductDto>
 {
     public Guid Id { get; set; }

    
 }

 public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
 {
     private readonly IProductService _productService;
     private readonly IMapper _mapper;

     public GetProductByIdQueryHandler(IProductService productService, IMapper mapper)
     {
         _productService = productService;
         _mapper = mapper;
     }

     public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
     {
         //var product = await _productService.GetByIdAsync(request.Id);
         //return _mapper.Map<ProductDto>(product);
         return new ProductDto
         {
             Id = request.Id,
             Name = "Sample Product",
             Description = "This is a sample product description.",
             Price = 99.99m,
             IsActive = true,
         }; // Placeholder for actual implementation
    }
 }