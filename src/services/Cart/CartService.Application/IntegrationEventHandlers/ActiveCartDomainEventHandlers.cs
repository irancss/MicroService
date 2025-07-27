using BuildingBlocks.Application.CQRS.DomainEvents;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Interfaces;
using Cart.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cart.Application.IntegrationEventHandlers;
/// <summary>
/// A generic handler that listens to any domain event from ActiveCart
/// and publishes a unified integration event representing the final state of the cart.
/// This simplifies logic by having one point of contact for external services.
/// </summary>
public class ActiveCartChangedHandler :
    INotificationHandler<DomainNotificationBase<ItemAddedToActiveCartEvent>>,
    INotificationHandler<DomainNotificationBase<ItemRemovedFromActiveCartEvent>>,
    INotificationHandler<DomainNotificationBase<ItemQuantityUpdatedInActiveCartEvent>>,
    INotificationHandler<DomainNotificationBase<ActiveCartClearedEvent>>,
    INotificationHandler<DomainNotificationBase<CartsMergedEvent>>
{
    private readonly IEventBus _eventBus;
    private readonly IActiveCartRepository _cartRepository;
    private readonly ILogger<ActiveCartChangedHandler> _logger;

    public ActiveCartChangedHandler(IEventBus eventBus, IActiveCartRepository cartRepository, ILogger<ActiveCartChangedHandler> logger)
    {
        _eventBus = eventBus;
        _cartRepository = cartRepository;
        _logger = logger;
    }

    // A single, generic method to handle publishing
    private async Task PublishCartUpdateAsync(string cartId, string? userId, string eventType, CancellationToken cancellationToken)
    {
        var finalCartState = await _cartRepository.GetByIdAsync(cartId, cancellationToken);

        var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
            finalCartState?.UserId ?? userId,
            finalCartState?.Id ?? cartId,
            finalCartState?.TotalItems ?? 0,
            finalCartState?.TotalPrice ?? 0m,
            finalCartState?.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList() ?? new List<CartItemDetails>()
        );

        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        _logger.LogInformation(
            "Domain Event Handled: {DomainEventType}. Published Integration Event: {IntegrationEventType} for CartId {CartId}",
            eventType,
            nameof(ActiveCartUpdatedIntegrationEvent),
            cartId);
    }

    // Implement Handle for each domain event, delegating to the generic method
    public Task Handle(DomainNotificationBase<ItemAddedToActiveCartEvent> notification, CancellationToken cancellationToken) =>
        PublishCartUpdateAsync(notification.DomainEvent.CartId, notification.DomainEvent.UserId, nameof(ItemAddedToActiveCartEvent), cancellationToken);

    public Task Handle(DomainNotificationBase<ItemRemovedFromActiveCartEvent> notification, CancellationToken cancellationToken) =>
        PublishCartUpdateAsync(notification.DomainEvent.CartId, notification.DomainEvent.UserId, nameof(ItemRemovedFromActiveCartEvent), cancellationToken);

    public Task Handle(DomainNotificationBase<ItemQuantityUpdatedInActiveCartEvent> notification, CancellationToken cancellationToken) =>
        PublishCartUpdateAsync(notification.DomainEvent.CartId, notification.DomainEvent.UserId, nameof(ItemQuantityUpdatedInActiveCartEvent), cancellationToken);

    public Task Handle(DomainNotificationBase<ActiveCartClearedEvent> notification, CancellationToken cancellationToken) =>
        PublishCartUpdateAsync(notification.DomainEvent.CartId, notification.DomainEvent.UserId, nameof(ActiveCartClearedEvent), cancellationToken);

    public Task Handle(DomainNotificationBase<CartsMergedEvent> notification, CancellationToken cancellationToken) =>
        PublishCartUpdateAsync(notification.DomainEvent.UserCartId, notification.DomainEvent.UserId, nameof(CartsMergedEvent), cancellationToken);
}