

using BuildingBlocks.Messaging.Events.Base;

namespace ProductService.Domain.Events
{
    public record VendorCreatedEvent : IntegrationEvent
    {
        public string VendorId { get; }
        public string VendorName { get; }

        public VendorCreatedEvent(string vendorId, string vendorName)
        {
            VendorId = vendorId;
            VendorName = vendorName;
        }
    }
}

