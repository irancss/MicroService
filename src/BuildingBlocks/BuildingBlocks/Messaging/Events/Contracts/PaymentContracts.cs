using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Contracts
{
    public record PaymentRequestEvent(int OrderId, decimal Amount) : IntegrationEvent;

    public record PaymentSucceededEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public string PaymentId { get; init; } = string.Empty;
    }

    public record PaymentFailedEvent : IntegrationEvent
    {
        public int OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
    }
}