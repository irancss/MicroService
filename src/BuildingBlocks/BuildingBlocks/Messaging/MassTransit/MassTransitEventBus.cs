using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Base;
using MassTransit;
using System.Linq; // [مهم] اضافه کردن این using برای استفاده از .Select()
using System.Threading.Tasks; // [مهم] اضافه کردن این using برای استفاده از Task.WhenAll

namespace BuildingBlocks.Messaging.MassTransit
{
    /// <summary>
    /// A MassTransit-based implementation of the IEventBus interface.
    /// </summary>
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            // انتشار یک رویداد واحد
            return _publishEndpoint.Publish(@event, cancellationToken);
        }

        public Task PublishAsync<T>(IEnumerable<T> events, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            // [اصلاح شد] راه حل صحیح برای MassTransit 8 و بالاتر
            // متد PublishMultiple حذف شده است. بهترین راه، ارسال موازی آنها با Task.WhenAll است.
            if (events == null || !events.Any())
            {
                return Task.CompletedTask;
            }

            // یک لیست از تسک‌های پابلیش ایجاد می‌کنیم
            var publishTasks = events.Select(e => _publishEndpoint.Publish(e, cancellationToken));

            // و منتظر می‌مانیم تا همه آنها تمام شوند
            return Task.WhenAll(publishTasks);
        }
    }
}