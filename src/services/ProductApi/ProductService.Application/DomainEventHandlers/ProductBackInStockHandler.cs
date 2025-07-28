using BuildingBlocks.Application.CQRS.DomainEvents;
using ProductService.Domain.Events;
using BuildingBlocks.Messaging.Abstractions;

namespace ProductService.Application.DomainEventHandlers
{
    // این Handler به رویداد دامنه گوش می‌دهد
    public class ProductBackInStockHandler : DomainNotificationBase<ProductBackInStockDomainEvent>
    {
        private readonly IEventBus _eventBus;

        //public ProductBackInStockHandler(IEventBus eventBus)
        //{
        //    _eventBus = eventBus;
        //}

        public ProductBackInStockHandler(ProductBackInStockDomainEvent domainEvent, IEventBus eventBus) : base(domainEvent)
        {
            _eventBus = eventBus;
        }

        public async Task Handle(ProductBackInStockDomainEvent notification, CancellationToken cancellationToken)
        {
            // و یک رویداد یکپارچه‌سازی برای سایر سرویس‌ها منتشر می‌کند
            var integrationEvent = new ProductBecameAvailableIntegrationEvent(
                notification.ProductId,
                notification.ProductName
            );

            await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
