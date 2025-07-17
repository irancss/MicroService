using MassTransit;

namespace BuildingBlocks.Messaging.Events
{
    public abstract class BaseEvent
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
        public string EventType { get; protected set; }

        protected BaseEvent()
        {
            EventType = GetType().Name;
        }
    }

    public interface IEventHandler<in TEvent> : IConsumer<TEvent>
        where TEvent : class
    {
    }

    // Sample Events
    public class OrderCreatedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
    }

    public class PaymentProcessedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public bool IsSuccessful { get; set; }
        public string PaymentId { get; set; } = string.Empty;
    }

    public class InventoryReservedEvent : BaseEvent
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public string ReservationId { get; set; } = string.Empty;
    }
}
