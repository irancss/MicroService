using BuildingBlocks.Messaging.Events.Base;
using BuildingBlocks.Messaging.Events.Contracts;

namespace BuildingBlocks.Messaging.Events.Domains
{
    public record OrderCreatedEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public DateTime OrderDate { get; init; } = DateTime.Now;
        public List<OrderItemData> Items { get; init; } = new();
    }

    public record OrderCancellationEvent(int OrderId, string Reason) : IntegrationEvent;

    public record OrderStatusChangedEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public string NewStatus { get; init; } = string.Empty;
    }
}