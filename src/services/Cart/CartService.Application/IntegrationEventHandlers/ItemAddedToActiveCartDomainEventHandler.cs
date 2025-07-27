using BuildingBlocks.Application.CQRS.DomainEvents;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Interfaces;
using Cart.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cart.Application.IntegrationEventHandlers;
public class ItemAddedToActiveCartDomainEventHandler : INotificationHandler<DomainNotificationBase<ItemAddedToActiveCartEvent>>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<ItemAddedToActiveCartDomainEventHandler> _logger;
    private readonly IActiveCartRepository _cartRepository; // برای گرفتن وضعیت نهایی سبد

    public ItemAddedToActiveCartDomainEventHandler(IEventBus eventBus, ILogger<ItemAddedToActiveCartDomainEventHandler> logger, IActiveCartRepository cartRepository)
    {
        _eventBus = eventBus;
        _logger = logger;
        _cartRepository = cartRepository;
    }

    // [اصلاح شد] امضای متد Handle مطابق با INotificationHandler است
    public async Task Handle(DomainNotificationBase<ItemAddedToActiveCartEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        // وضعیت نهایی سبد را از Redis بخوان
        var finalCartState = await _cartRepository.GetByIdAsync(domainEvent.CartId, cancellationToken);
        if (finalCartState == null)
        {
            _logger.LogError("Cannot publish integration event. Cart with ID {CartId} not found after domain event.", domainEvent.CartId);
            return;
        }

        // رویداد یکپارچه‌سازی (Integration Event) را بساز
        var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
            finalCartState.UserId,
            finalCartState.Id,
            finalCartState.TotalItems,
            finalCartState.TotalPrice,
            finalCartState.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
        );

        // و آن را از طریق Event Bus منتشر کن (که از طریق Outbox ارسال خواهد شد)
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "Domain Event Handled: {DomainEvent}. Published Integration Event: {IntegrationEvent}",
            nameof(ItemAddedToActiveCartEvent),
            nameof(ActiveCartUpdatedIntegrationEvent));
    }
}