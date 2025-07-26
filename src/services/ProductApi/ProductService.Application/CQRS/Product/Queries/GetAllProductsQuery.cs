using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.DTOs.Product;

namespace ProductService.Application.CQRS.Product.Queries;

public class GetAllProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetAllProductsQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        //var products = await _productService.GetAllAsync(request.PageNumber, request.PageSize);
        //return _mapper.Map<PaginatedList<ProductDto>>(products);
        return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, request.PageNumber, request.PageSize); // Placeholder for actual implementation
    }
}