using BuildingBlocks.Abstractions;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SaveItemForLaterHandler> _logger;

        public SaveItemForLaterHandler(
            IActiveCartRepository activeCartRepo,
            INextPurchaseCartRepository nextPurchaseRepo,
            IUnitOfWork unitOfWork,
            ILogger<SaveItemForLaterHandler> logger)
        {
            _activeCartRepo = activeCartRepo;
            _nextPurchaseRepo = nextPurchaseRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CartOperationResultDto> Handle(SaveItemForLaterCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the active cart
            var activeCart = await _activeCartRepo.GetByIdAsync(request.UserId, cancellationToken);
            if (activeCart is null)
            {
                return new CartOperationResultDto(false, "Active cart not found.", null);
            }

            // 2. Find the item to move
            var itemToMove = activeCart.FindItem(request.ProductId, request.VariantId);
            if (itemToMove is null)
            {
                return new CartOperationResultDto(false, "Item not found in active cart.", activeCart.ToDto());
            }

            // 3. Get or create the next-purchase cart
            var nextPurchaseCart = await _nextPurchaseRepo.GetByUserIdAsync(request.UserId, cancellationToken)
                                   ?? NextPurchaseCart.Create(request.UserId);

            // 4. Create the item for the next-purchase cart
            var newItemForLater = NextPurchaseItem.Create(
                itemToMove.ProductId,
                itemToMove.ProductName,
                itemToMove.PriceAtTimeOfAddition,
                itemToMove.ProductImageUrl,
                itemToMove.VariantId
            );

            // 5. Add to next-purchase cart (this will raise a domain event)
            nextPurchaseCart.AddItem(newItemForLater);

            // 6. Remove from active cart (this will raise a domain event)
            activeCart.RemoveItem(itemToMove);

            // 7. Save both carts
            // Save the persistent cart within a transaction
            await _nextPurchaseRepo.SaveAsync(nextPurchaseCart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken); // This commits the NextPurchaseCart changes transactionally

            // Save the cache-based cart
            await _activeCartRepo.SaveAsync(activeCart, cancellationToken);

            _logger.LogInformation("Item {ProductId} for user {UserId} was saved for later.", request.ProductId, request.UserId);

            // Domain Event Handlers will now automatically publish the necessary integration events.
            
            return new CartOperationResultDto(true, "Item successfully saved for later.", activeCart.ToDto());
        }
    }
}
