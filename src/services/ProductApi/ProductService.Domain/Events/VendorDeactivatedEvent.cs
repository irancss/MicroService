using ProductService.Domain.Common;

namespace ProductService.Domain.Events;

public class VendorDeactivatedEvent : BaseEvent
{
    public string VendorId { get; }
    public VendorDeactivatedEvent(string vendorId)
    {
        VendorId = vendorId;
    }
}