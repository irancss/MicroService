using BuildingBlocks.Messaging.Events.Base;
using BuildingBlocks.Domain.Events;

namespace ProductService.Domain.Events
{
    // --- Domain Events (برای استفاده داخلی در سرویس) ---

    // [اصلاح شد] ارث‌بری از DomainEvent به جای IDomainEvent
    public record ProductCreatedDomainEvent(Guid ProductId, string ProductName, string Sku, decimal Price, int Stock) : DomainEvent;

    public record ProductUpdatedDomainEvent(Guid ProductId) : DomainEvent;

    public record ProductBackInStockDomainEvent(Guid ProductId, string ProductName) : DomainEvent;

    public record ProductPriceUpdatedDomainEvent(Guid ProductId, decimal NewPrice, decimal OldPrice) : DomainEvent;


    // --- Integration Events (برای انتشار در Message Bus) ---

    // این بخش صحیح بود و نیازی به تغییر ندارد چون از IntegrationEvent ارث‌بری می‌کند.
    public record ProductBecameAvailableIntegrationEvent(Guid ProductId, string ProductName) : IntegrationEvent;
}
