using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands;

public class MoveItemToActiveCartHandler : ICommandHandler<MoveItemToActiveCartCommand, CartOperationResultDto>
{
    private readonly IActiveCartRepository _activeCartRepo;
    private readonly INextPurchaseCartRepository _nextPurchaseRepo;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MoveItemToActiveCartHandler> _logger;

    public MoveItemToActiveCartHandler(
        IActiveCartRepository activeCartRepo,
        INextPurchaseCartRepository nextPurchaseRepo,
        IEventBus eventBus,
        ILogger<MoveItemToActiveCartHandler> logger)
    {
        _activeCartRepo = activeCartRepo;
        _nextPurchaseRepo = nextPurchaseRepo;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(MoveItemToActiveCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the next-purchase cart from PostgreSQL
        var nextPurchaseCart = await _nextPurchaseRepo.GetByUserIdAsync(request.UserId, cancellationToken);
        if (nextPurchaseCart is null)
        {
            return new CartOperationResultDto(false, "Next-purchase cart not found.", null);
        }

        // 2. Find the item to move
        var itemToMove = nextPurchaseCart.FindItem(request.ProductId, request.VariantId);
        if (itemToMove is null)
        {
            return new CartOperationResultDto(false, "Item not found in next-purchase cart.", null);
        }

        // 3. Get or create the active cart from Redis
        var activeCart = await _activeCartRepo.GetByIdAsync(request.UserId, cancellationToken)
                         ?? Domain.Entities.ActiveCart.Create(request.UserId, request.UserId);

        // 4. Create the item for the active cart and add it
        // We assume quantity is always 1 when moving from "saved for later"
        var newItemForActiveCart = Domain.Entities.CartItem.Create(
            itemToMove.ProductId,
            itemToMove.ProductName,
            1, // Default quantity when moving back to active cart
            itemToMove.SavedPrice,
            itemToMove.ProductImageUrl,
            itemToMove.VariantId
        );
        activeCart.AddItem(newItemForActiveCart);

        // 5. Remove the item from the next-purchase cart
        nextPurchaseCart.RemoveItem(itemToMove);

        // 6. Save both carts
        await _nextPurchaseRepo.SaveAsync(nextPurchaseCart, cancellationToken);
        await _activeCartRepo.SaveAsync(activeCart, cancellationToken);

        _logger.LogInformation("Item {ProductId} moved to active cart for user {UserId}", request.ProductId, request.UserId);

        // 7. Publish integration events
        var activeCartEvent = new ActiveCartUpdatedIntegrationEvent(
            activeCart.UserId, activeCart.Id, activeCart.TotalItems, activeCart.TotalPrice,
            activeCart.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
        );

        // We might also want a specific event for this action for analytics
        var movedToActiveEvent = new ItemMovedToActiveCartIntegrationEvent(
            request.UserId, request.ProductId, itemToMove.SavedPrice
        );

        await _eventBus.PublishAsync(activeCartEvent, cancellationToken);
        await _eventBus.PublishAsync(movedToActiveEvent, cancellationToken);

        return new CartOperationResultDto(true, "Item successfully moved to active cart.", activeCart.ToDto());
    }
}