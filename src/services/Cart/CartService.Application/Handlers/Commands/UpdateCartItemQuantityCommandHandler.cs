using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands;
public class UpdateItemQuantityInActiveCartHandler : ICommandHandler<UpdateItemQuantityInActiveCartCommand, CartOperationResultDto>
{
    private readonly IActiveCartRepository _cartRepository;
    private readonly IInventoryClient _inventoryClient;
    private readonly ILogger<UpdateItemQuantityInActiveCartHandler> _logger;

    public UpdateItemQuantityInActiveCartHandler(
        IActiveCartRepository cartRepository,
        IInventoryClient inventoryClient,
        ILogger<UpdateItemQuantityInActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(UpdateItemQuantityInActiveCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
            return new CartOperationResultDto(false, "Cart not found.", null);

        // بررسی موجودی
        if (request.NewQuantity > 0 && !await _inventoryClient.CheckStockAvailabilityAsync(request.ProductId, request.NewQuantity, cancellationToken))
            return new CartOperationResultDto(false, "Insufficient stock for the requested quantity.", cart.ToDto());
        
        // فراخوانی متد Aggregate که خودش رویداد دامنه را ایجاد می‌کند
        cart.UpdateItemQuantity(request.ProductId, request.VariantId, request.NewQuantity);

        await _cartRepository.SaveAsync(cart, cancellationToken);

        _logger.LogInformation("Updated quantity for item {ProductId} in cart {CartId}", request.ProductId, request.CartId);

        // دیگر نیازی به انتشار رویداد در اینجا نیست

        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}