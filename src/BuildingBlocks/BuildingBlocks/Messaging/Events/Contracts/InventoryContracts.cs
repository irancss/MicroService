using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Contracts
{
    /// <summary>
    /// A simple DTO for order items, used in various events.
    /// </summary>
    public record struct OrderItemData(int ProductId, int Quantity);

    /// <summary>
    /// Published by a saga to request reservation of inventory for an order.
    /// Consumed by the Inventory service.
    /// </summary>
    public record InventoryReservationRequestEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public List<OrderItemData> ItemsToReserve { get; init; } = new();
    }

    /// <summary>
    /// Published by the Inventory service when items for an order have been successfully reserved.
    /// Consumed by the Order saga to proceed to the payment step.
    /// </summary>
    public record InventoryReservedEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
    }

    /// <summary>
    /// Published by the Inventory service when reservation fails (e.g., item out of stock).
    /// Consumed by the Order saga to cancel the order.
    /// </summary>
    public record InventoryReservationFailedEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public List<int> FailedProductIds { get; init; } = new();
    }

    /// <summary>
    /// Published by a saga as a compensation action to release previously reserved inventory.
    /// Consumed by the Inventory service.
    /// </summary>
    public record InventoryReleaseRequestEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public List<OrderItemData> ItemsToRelease { get; init; } = new();
        public string Reason { get; init; } = "Order Canceled";
    }
}