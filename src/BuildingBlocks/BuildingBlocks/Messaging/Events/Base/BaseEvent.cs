using MassTransit;

namespace BuildingBlocks.Messaging.Events.Base
{
    /// <summary>
    /// [اصلاح شد] نام این رکورد به IntegrationEvent تغییر یافت تا هدف آن (ارتباط بین سرویس‌ها) واضح‌تر باشد.
    /// This is the base record for all events that are published across service boundaries.
    /// </summary>
    public abstract record IntegrationEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
        public Guid CorrelationId { get; set; } = NewId.NextGuid();
    }

    /// <summary>
    /// A marker interface for event handlers (consumers) in MassTransit.
    /// </summary>
    public interface IEventHandler<in TEvent> : IConsumer<TEvent>
        where TEvent : IntegrationEvent
    {
    }
}