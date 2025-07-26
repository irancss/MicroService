using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public class GetVendorCountQuery : IRequest<int>
{
    public GetVendorCountQuery()
    {
    }
}

public class GetVendorCountQueryHandler : IRequestHandler<GetVendorCountQuery, int>
{
    private readonly IVendorService _vendorService;

    public GetVendorCountQueryHandler(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    public async Task<int> Handle(GetVendorCountQuery request, CancellationToken cancellationToken)
    {
       // return await _vendorService.GetVendorCountAsync();
       return 1; // Placeholder return value, replace with actual implementation
    }
}
