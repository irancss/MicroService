using ProductService.Domain.Common;

namespace ProductService.Domain.Events;

public class VendorActivatedEvent : BaseEvent
{
    public string VendorId { get; }
    public VendorActivatedEvent(string vendorId)
    {
        VendorId = vendorId;
    }
}