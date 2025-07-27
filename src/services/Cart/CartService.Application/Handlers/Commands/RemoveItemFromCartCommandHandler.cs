using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands;

public class RemoveItemFromActiveCartHandler : ICommandHandler<RemoveItemFromActiveCartCommand, CartOperationResultDto>
{
    private readonly IActiveCartRepository _cartRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<RemoveItemFromActiveCartHandler> _logger;

    public RemoveItemFromActiveCartHandler(
        IActiveCartRepository cartRepository,
        IEventBus eventBus,
        ILogger<RemoveItemFromActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(RemoveItemFromActiveCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
        {
            // If cart doesn't exist, it's already "empty", so return success.
            return new CartOperationResultDto(true, null, CartDto.CreateEmpty(request.CartId, request.UserId));
        }

        var itemToRemove = cart.FindItem(request.ProductId, request.VariantId);
        if (itemToRemove is null)
        {
            // Item is not in the cart, which is a success state for a remove operation.
            _logger.LogWarning("Attempted to remove item {ProductId} which was not in cart {CartId}.", request.ProductId, request.CartId);
            return new CartOperationResultDto(true, "Item was not in the cart.", cart.ToDto());
        }

        cart.RemoveItem(itemToRemove);

        await _cartRepository.SaveAsync(cart, cancellationToken);

        _logger.LogInformation("Item {ProductId} removed from cart {CartId}", request.ProductId, request.CartId);

        // Publish an integration event to notify other services (e.g., Inventory to release stock)
        var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
            cart.UserId,
            cart.Id,
            cart.TotalItems,
            cart.TotalPrice,
            cart.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
        );
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}