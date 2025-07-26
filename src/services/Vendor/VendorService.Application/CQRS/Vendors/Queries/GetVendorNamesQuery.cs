using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Queries;

public class GetVendorNamesQuery : IRequest<List<string>>
{
    public string? SearchTerm { get; set; }

    public GetVendorNamesQuery(string? searchTerm = null)
    {
        SearchTerm = searchTerm;
    }
}

public class GetVendorNamesQueryHandler : IRequestHandler<GetVendorNamesQuery, List<string>>
{
    private readonly IVendorService _vendorService;

    public GetVendorNamesQueryHandler(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    public async Task<List<string>> Handle(GetVendorNamesQuery request, CancellationToken cancellationToken)
    {
       // return await _vendorService.GetVendorNamesAsync(request.SearchTerm, cancellationToken);
       return new List<string>() { "Vendor1", "Vendor2", "Vendor3" }; // Placeholder for actual implementation
    }
}