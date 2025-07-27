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
    private readonly ILogger<RemoveItemFromActiveCartHandler> _logger;

    public RemoveItemFromActiveCartHandler(
        IActiveCartRepository cartRepository,
        ILogger<RemoveItemFromActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
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

         cart.RemoveItem(itemToRemove); // این متد خودش رویداد دامنه را ایجاد می‌کند

        await _cartRepository.SaveAsync(cart, cancellationToken);
        _logger.LogInformation("Item {ProductId} removed from cart {CartId}", request.ProductId, request.CartId);

        // فراخوانی مستقیم IEventBus حذف می‌شود
        
        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}