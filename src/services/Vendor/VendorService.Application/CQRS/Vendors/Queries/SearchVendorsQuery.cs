using AutoMapper;
using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public class SearchVendorsQuery : IRequest<IEnumerable<VendorDto>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public SearchVendorsQuery(string? searchTerm, int pageNumber, int pageSize)
    {
        SearchTerm = searchTerm;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
public class SearchVendorsQueryHandler : IRequestHandler<SearchVendorsQuery, IEnumerable<VendorDto>>
{
    private readonly IVendorService _vendorService;
    private readonly IMapper _mapper;

    public SearchVendorsQueryHandler(IVendorService vendorService, IMapper mapper)
    {
        _vendorService = vendorService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VendorDto>> Handle(SearchVendorsQuery request, CancellationToken cancellationToken)
    {
        //var vendors = await _vendorService.SearchVendorsAsync(request.SearchTerm, request.PageNumber, request.PageSize);
        //return _mapper.Map<IEnumerable<VendorDto>>(vendors);
        return new List<VendorDto>(); // Placeholder for actual implementation
    }
}