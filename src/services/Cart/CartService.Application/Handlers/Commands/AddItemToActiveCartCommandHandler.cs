using BuildingBlocks.Application.CQRS.Commands;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands;

public class AddItemToActiveCartHandler : ICommandHandler<AddItemToActiveCartCommand, CartOperationResultDto>
{
    private readonly IActiveCartRepository _cartRepository;
    private readonly ICatalogClient _catalogClient;
    private readonly IInventoryClient _inventoryClient;
    // IEventBus و IUnitOfWork دیگر در این Handler لازم نیستند
    private readonly ILogger<AddItemToActiveCartHandler> _logger;

    public AddItemToActiveCartHandler(
        IActiveCartRepository cartRepository,
        ICatalogClient catalogClient,
        IInventoryClient inventoryClient,
        ILogger<AddItemToActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _catalogClient = catalogClient;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(AddItemToActiveCartCommand request, CancellationToken cancellationToken)
    {
        // 1. اعتبارسنجی محصول، قیمت و موجودی (بدون تغییر)
        var productInfo = await _catalogClient.GetProductInfoAsync(request.ProductId, cancellationToken);
        if (productInfo is null || !productInfo.IsActive)
            return new CartOperationResultDto(false, "Product not found or is inactive.", null);

        var price = await _inventoryClient.GetCurrentPriceAsync(request.ProductId, cancellationToken);
        if (price is null)
            return new CartOperationResultDto(false, "Could not retrieve product price.", null);

        if (!await _inventoryClient.CheckStockAvailabilityAsync(request.ProductId, request.Quantity, cancellationToken))
            return new CartOperationResultDto(false, "Product is out of stock.", null);

        // 2. [جدید] ابتدا تلاش برای رزرو موجودی
        try
        {
            // این یک Command مستقیم به سرویس Inventory است
            await _inventoryClient.ReserveStockAsync(request.CartId, new() { { request.ProductId, request.Quantity } }, cancellationToken);
            _logger.LogInformation("Stock reserved for Product {ProductId} in Cart {CartId}", request.ProductId, request.CartId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve stock for Product {ProductId}. Aborting add to cart.", request.ProductId);
            // اگر رزرو موجودی با خطا مواجه شود، آیتم به سبد اضافه نمی‌شود.
            return new CartOperationResultDto(false, "Could not reserve stock for the product.", null);
        }

        // 3. اگر رزرو موفق بود، سبد را در Redis به‌روز کن
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken)
                   ?? ActiveCart.Create(request.CartId, request.UserId);

        var newItem = CartItem.Create(request.ProductId, productInfo.Name, request.Quantity, price.Value, productInfo.ImageUrl, request.VariantId);
        cart.AddItem(newItem);

        await _cartRepository.SaveAsync(cart, cancellationToken);
        _logger.LogInformation("Item {ProductId} added to cart {CartId} in Redis.", request.ProductId, request.CartId);

        // 4. [نکته] ما دیگر رویدادی منتشر نمی‌کنیم.
        // سرویس Inventory خودش مسئول مدیریت رزرو است.
        // اگر بخواهیم سرویس‌های دیگر را مطلع کنیم، باید از طریق Domain Event Handler که در بالا ساختیم عمل کنیم.
        // اما در این سناریو خاص، چون فقط Inventory مهم است، نیازی به انتشار رویداد اضافی نیست.

        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}