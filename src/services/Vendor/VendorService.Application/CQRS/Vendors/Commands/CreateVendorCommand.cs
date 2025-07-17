
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Commands;

using AutoMapper;
using MediatR;

public record CreateVendorCommand(
    string Name, 
    string NationalId,
    string Street, 
    string City,
    string Province, 
    string PostalCode,
    string PhoneNumber, 
    string Email) : IRequest<string>;


public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, string>
{
    IVendorService _vendorService;
    IMapper _mapper;
    public CreateVendorCommandHandler(IVendorService vendorService, IMapper mapper)
    {
        _mapper = mapper;
        _vendorService = vendorService;
    }

    public async Task<string> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = _mapper.Map<VendorDto>(request);
        var vendorId = await _vendorService.CreateVendorAsync(vendor);
        return vendorId;
    }
}