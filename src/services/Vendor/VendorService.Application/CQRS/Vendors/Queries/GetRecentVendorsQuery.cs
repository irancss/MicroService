using AutoMapper;
using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public class GetRecentVendorsQuery : IRequest<List<VendorDto>>
{
    public int Count { get; set; }

    public GetRecentVendorsQuery(int count)
    {
        Count = count;
    }
}

public class GetRecentVendorsQueryHandler : IRequestHandler<GetRecentVendorsQuery, List<VendorDto>>
{
    private readonly IVendorService _vendorService;
    private readonly IMapper _mapper;

    public GetRecentVendorsQueryHandler(IVendorService vendorService, IMapper mapper)
    {
        _vendorService = vendorService;
        _mapper = mapper;
    }

    public async Task<List<VendorDto>> Handle(GetRecentVendorsQuery request, CancellationToken cancellationToken)
    {
        //var vendors = await _vendorService.GetRecentVendorsAsync(request.Count);
        //return _mapper.Map<List<VendorDto>>(vendors);
        return new List<VendorDto>(); // Placeholder for actual implementation
    }
}
