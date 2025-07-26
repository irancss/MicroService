using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Abstractions
{
    /// <summary>
    /// [اصلاح شد] اینترفیس اصلی برای انتشار رویدادهای یکپارچه‌سازی (Integration Events).
    /// این اینترفیس توسط سرویس‌ها برای ارتباط غیرهمگام با یکدیگر استفاده می‌شود.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes a single event to the message bus.
        /// </summary>
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;

        /// <summary>
        /// Publishes a collection of events to the message bus.
        /// </summary>
        Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    }
}