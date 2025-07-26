using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.DTOs.Product;

namespace ProductService.Application.CQRS.Product.Queries;

public class SearchProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public string? Query { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public SearchProductsQuery(string? query, int pageNumber, int pageSize)
    {
        Query = query ?? string.Empty;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        //var products = await _productService.SearchAsync(request.Query, request.PageNumber, request.PageSize);
        //return _mapper.Map<PaginatedList<ProductDto>>(products);
        return new PaginatedList<ProductDto>(new List<ProductDto>(), 0, request.PageNumber, request.PageSize); // Placeholder for actual implementation
    }
}