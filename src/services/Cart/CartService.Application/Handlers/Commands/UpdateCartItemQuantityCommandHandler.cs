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
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly IEventBus _eventBus;
    private readonly ILogger<UpdateItemQuantityInActiveCartHandler> _logger;

    public UpdateItemQuantityInActiveCartHandler(
        IActiveCartRepository cartRepository,
        IInventoryGrpcClient inventoryClient,
        IEventBus eventBus,
        ILogger<UpdateItemQuantityInActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _inventoryClient = inventoryClient;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(UpdateItemQuantityInActiveCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
        {
            return new CartOperationResultDto(false, "Cart not found.", null);
        }

        var itemToUpdate = cart.FindItem(request.ProductId, request.VariantId);
        if (itemToUpdate is null)
        {
            return new CartOperationResultDto(false, "Item not found in cart.", null);
        }

        // اگر تعداد جدید صفر یا منفی است، آیتم را حذف کن
        if (request.NewQuantity <= 0)
        {
            cart.RemoveItem(itemToUpdate);
        }
        else
        {
            // بررسی موجودی برای تعداد جدید
            if (!await _inventoryClient.CheckStockAvailabilityAsync(request.ProductId, request.NewQuantity, cancellationToken))
            {
                return new CartOperationResultDto(false, "Insufficient stock for the requested quantity.", cart.ToDto());
            }

            // آیتم‌های ما Immutable هستند، پس آیتم قدیمی را حذف و جدید را با تعداد جدید اضافه می‌کنیم.
            var updatedItem = Cart.Domain.Entities.CartItem.Create(
                itemToUpdate.ProductId,
                itemToUpdate.ProductName,
                request.NewQuantity,
                itemToUpdate.PriceAtTimeOfAddition,
                itemToUpdate.ProductImageUrl,
                itemToUpdate.VariantId
            );

            cart.RemoveItem(itemToUpdate);
            cart.AddItem(updatedItem);
        }

        await _cartRepository.SaveAsync(cart, cancellationToken);

        _logger.LogInformation("Updated quantity for item {ProductId} in cart {CartId}", request.ProductId, request.CartId);

        // انتشار رویداد برای به‌روزرسانی رزرو موجودی
        var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
            cart.UserId, cart.Id, cart.TotalItems, cart.TotalPrice,
            cart.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
        );
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);
        // (منطق رزرو موجودی باید در Consumer این رویداد در سرویس Inventory باشد)

        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}