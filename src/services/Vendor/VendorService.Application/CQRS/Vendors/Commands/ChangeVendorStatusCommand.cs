using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Commands;

public class ChangeVendorStatusCommand : IRequest<bool>
{
    public string VendorId { get; set; }
    public bool IsActive { get; set; }

    public ChangeVendorStatusCommand(string vendorId, bool isActive)
    {
        VendorId = vendorId;
        IsActive = isActive;
    }
}

public class ChangeVendorStatusCommandHandler : IRequestHandler<ChangeVendorStatusCommand, bool>
{
    private readonly IVendorService _vendorService;

    public ChangeVendorStatusCommandHandler(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    public async Task<bool> Handle(ChangeVendorStatusCommand request, CancellationToken cancellationToken)
    {
        return await _vendorService.ChangeVendorStatusAsync(request.VendorId, request.IsActive);
    }
}