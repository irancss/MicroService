using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Contracts
{
    public record ShipmentRequestEvent(int OrderId, string ShippingAddress) : IntegrationEvent;

    public record ShipmentCreatedEvent : IntegrationEvent
    {
        public int ShipmentId { get; init; }
        public int OrderId { get; init; }
        public string TrackingNumber { get; init; } = string.Empty;
        public string Carrier { get; init; } = string.Empty;
        public DateTime EstimatedDeliveryDate { get; init; }
    }

    public record ShipmentStatusChangedEvent : IntegrationEvent
    {
        public int ShipmentId { get; init; }
        public int OrderId { get; init; }
        public string NewStatus { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public DateTime StatusUpdateTimestamp { get; init; }
    }
}