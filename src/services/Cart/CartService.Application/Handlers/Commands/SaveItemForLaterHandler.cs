using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands
{
    public class SaveItemForLaterHandler : ICommandHandler<SaveItemForLaterCommand, CartOperationResultDto>
    {
        private readonly IActiveCartRepository _activeCartRepo;
        private readonly INextPurchaseCartRepository _nextPurchaseRepo;
        private readonly IEventBus _eventBus;
        private readonly ILogger<SaveItemForLaterHandler> _logger;

        public SaveItemForLaterHandler(
            IActiveCartRepository activeCartRepo,
            INextPurchaseCartRepository nextPurchaseRepo,
            IEventBus eventBus,
            ILogger<SaveItemForLaterHandler> logger)
        {
            _activeCartRepo = activeCartRepo;
            _nextPurchaseRepo = nextPurchaseRepo;
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task<CartOperationResultDto> Handle(SaveItemForLaterCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the active cart from Redis. UserId is also the CartId for active carts.
            var activeCart = await _activeCartRepo.GetByIdAsync(request.UserId, cancellationToken);
            if (activeCart is null)
            {
                return new CartOperationResultDto(false, "Active cart not found.", null);
            }

            // 2. Find the item to move from the active cart.
            var itemToMove = activeCart.FindItem(request.ProductId, request.VariantId);
            if (itemToMove is null)
            {
                return new CartOperationResultDto(false, "Item not found in active cart.", activeCart.ToDto());
            }

            // 3. Get or create the next-purchase cart from PostgreSQL.
            var nextPurchaseCart = await _nextPurchaseRepo.GetByUserIdAsync(request.UserId, cancellationToken)
                                   ?? NextPurchaseCart.Create(request.UserId);

            // 4. Create the item for the next-purchase cart and add it.
            var newItemForLater = NextPurchaseItem.Create(
                itemToMove.ProductId,
                itemToMove.ProductName,
                itemToMove.PriceAtTimeOfAddition,
                itemToMove.ProductImageUrl,
                itemToMove.VariantId
            );
            nextPurchaseCart.AddItem(newItemForLater);

            // 5. Remove the item from the active cart.
            activeCart.RemoveItem(itemToMove);

            // 6. Save both carts. We rely on eventual consistency.
            // First, save to the persistent store (PostgreSQL).
            await _nextPurchaseRepo.SaveAsync(nextPurchaseCart, cancellationToken);
            // Then, update the cache (Redis).
            await _activeCartRepo.SaveAsync(activeCart, cancellationToken);

            _logger.LogInformation("Item {ProductId} for user {UserId} was saved for later.", request.ProductId, request.UserId);

            // 7. Publish integration events.
            // Event 1: Notify that the active cart has changed. Inventory service will listen to this to release the stock reservation.
            var activeCartEvent = new ActiveCartUpdatedIntegrationEvent(
                activeCart.UserId, activeCart.Id, activeCart.TotalItems, activeCart.TotalPrice,
                activeCart.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
            );

            // Event 2: Notify that an item was saved for later (for analytics, marketing, etc.).
            var savedForLaterEvent = new ItemSavedForLaterIntegrationEvent(
                request.UserId, request.ProductId, itemToMove.PriceAtTimeOfAddition
            );

            // Publish both events.
            await _eventBus.PublishAsync(activeCartEvent, cancellationToken);
            await _eventBus.PublishAsync(savedForLaterEvent, cancellationToken);

            // 8. Return the updated active cart to the user.
            return new CartOperationResultDto(true, "Item successfully saved for later.", activeCart.ToDto());
        }
    }
}
