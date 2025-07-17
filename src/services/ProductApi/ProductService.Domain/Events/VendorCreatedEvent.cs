using ProductService.Domain.Common;

namespace ProductService.Domain.Events
{
    public class VendorCreatedEvent : BaseEvent
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

