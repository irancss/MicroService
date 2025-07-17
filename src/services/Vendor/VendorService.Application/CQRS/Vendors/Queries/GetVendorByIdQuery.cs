

using AutoMapper;
using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public record GetVendorByIdQuery(string Id) : IRequest<VendorDto?>;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorDto?>
{
    private readonly IVendorService _vendorService;
    private readonly IMapper _mapper;

    public GetVendorByIdQueryHandler(IVendorService vendorService, IMapper mapper)
    {
        _vendorService = vendorService;
        _mapper = mapper;
    }

    public async Task<VendorDto?> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorService.GetVendorByIdAsync(request.Id);
         if (vendor == null) return null;
        return _mapper.Map<VendorDto?>(vendor);
    }
}