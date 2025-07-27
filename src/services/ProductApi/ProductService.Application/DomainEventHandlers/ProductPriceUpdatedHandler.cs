using BuildingBlocks.Messaging.Events.Contracts;
using ProductService.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Messaging.Abstractions;

namespace ProductService.Application.DomainEventHandlers
{
    public class ProductPriceUpdatedHandler : IDomainEventHandler<ProductPriceUpdatedDomainEvent>
    {
        private readonly IEventBus _eventBus;

        public ProductPriceUpdatedHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task Handle(ProductPriceUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // تبدیل رویداد دامنه به رویداد یکپارچه‌سازی
            var integrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = notification.ProductId,
                NewPrice = notification.NewPrice,
                OldPrice = notification.OldPrice
            };

            // انتشار رویداد روی Message Bus برای سایر سرویس‌ها
            // الگوی Outbox که پیاده‌سازی کرده‌ایم، تضمین می‌کند که این پیام به صورت قابل اطمینان ارسال شود.
            await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
