using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Commands;

public class DeleteVendorCommand : IRequest<bool>
{
    public string VendorId { get; set; }

    public DeleteVendorCommand(string vendorId)
    {
        VendorId = vendorId;
    }
}

public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand, bool>
{
    private readonly IVendorService _vendorService;

    public DeleteVendorCommandHandler(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    public async Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _vendorService.DeleteVendorAsync(request.VendorId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}