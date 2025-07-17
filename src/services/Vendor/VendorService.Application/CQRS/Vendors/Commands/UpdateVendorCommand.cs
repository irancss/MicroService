using AutoMapper;
using MediatR;
using VendorService.application.Interfaces;

namespace VendorService.Application.CQRS.Vendors.Commands;

public class UpdateVendorCommand : IRequest<bool>
{
    public string VendorId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public bool IsActive { get; set; }

    public UpdateVendorCommand(string vendorId, string name, string description, string contactEmail, string contactPhone, bool isActive)
    {
        VendorId = vendorId;
        Name = name;
        Description = description;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        IsActive = isActive;
    }
}

public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand, bool>
{
    private readonly IVendorService _vendorService;
    private readonly IMapper _mapper;

    public UpdateVendorCommandHandler(IVendorService vendorService, IMapper mapper)
    {
        _vendorService = vendorService;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendorDto = _mapper.Map<VendorDto>(request);
        return await _vendorService.UpdateVendorAsync(vendorDto);
    }
}