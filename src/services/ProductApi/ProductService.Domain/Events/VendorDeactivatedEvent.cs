using BuildingBlocks.Messaging.Events.Base;

namespace ProductService.Domain.Events;

public record VendorDeactivatedEvent : IntegrationEvent
{
    public string VendorId { get; }
    public VendorDeactivatedEvent(string vendorId)
    {
        VendorId = vendorId;
    }
}