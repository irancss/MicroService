namespace ProductService.Application.CQRS.Product.Queries;
 
 public class GetProductByIdQuery : IRequest<ProductDto>
 {
     public string Id { get; set; }

    
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
         var product = await _productService.GetByIdAsync(request.Id);
         return _mapper.Map<ProductDto>(product);
     }
 }