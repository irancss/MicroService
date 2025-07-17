using AutoMapper;
using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public class GetTopRatedVendorsQuery : IRequest<List<VendorDto>>
{
    public int Count { get; }

    public GetTopRatedVendorsQuery(int count)
    {
        Count = count;
    }
}

public class GetTopRatedVendorsQueryHandler : IRequestHandler<GetTopRatedVendorsQuery, List<VendorDto>>
{
    private readonly IVendorService _vendorService;
    private readonly IMapper _mapper;

    public GetTopRatedVendorsQueryHandler(IVendorService vendorService, IMapper mapper)
    {
        _vendorService = vendorService;
        _mapper = mapper;
    }

    public async Task<List<VendorDto>> Handle(GetTopRatedVendorsQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _vendorService.GetTopRatedVendorsAsync(request.Count);
        return _mapper.Map<List<VendorDto>>(vendors);
    }
}