using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Messaging.Abstractions;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.IntegrationEventHandlers;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Cart.Application.Handlers.Commands;

public class AddItemToActiveCartHandler : ICommandHandler<AddItemToActiveCartCommand, CartOperationResultDto>
{
    private readonly IActiveCartRepository _cartRepository;
    private readonly ICatalogGrpcClient _catalogClient;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AddItemToActiveCartHandler> _logger;

    public AddItemToActiveCartHandler(
        IActiveCartRepository cartRepository,
        ICatalogGrpcClient catalogClient,
        IInventoryGrpcClient inventoryClient,
        IEventBus eventBus,
        ILogger<AddItemToActiveCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _catalogClient = catalogClient;
        _inventoryClient = inventoryClient;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CartOperationResultDto> Handle(AddItemToActiveCartCommand request, CancellationToken cancellationToken)
    {
        var productInfo = await _catalogClient.GetProductInfoAsync(request.ProductId, cancellationToken);
        if (productInfo is null || !productInfo.IsActive)
            return new CartOperationResultDto(false, "Product not found or is inactive.", null);

        var price = await _inventoryClient.GetCurrentPriceAsync(request.ProductId, cancellationToken);
        if (price is null)
            return new CartOperationResultDto(false, "Could not retrieve product price.", null);

        if (!await _inventoryClient.CheckStockAvailabilityAsync(request.ProductId, request.Quantity, cancellationToken))
            return new CartOperationResultDto(false, "Product is out of stock.", null);

        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken)
                   ?? ActiveCart.Create(request.CartId, request.UserId);

        var newItem = CartItem.Create(request.ProductId, productInfo.Name, request.Quantity, price.Value, productInfo.ImageUrl, request.VariantId);
        cart.AddItem(newItem);

        await _cartRepository.SaveAsync(cart, cancellationToken);

        _logger.LogInformation("Item {ProductId} added to cart {CartId}", request.ProductId, request.CartId);

        var integrationEvent = new ActiveCartUpdatedIntegrationEvent(
            cart.UserId, cart.Id, cart.TotalItems, cart.TotalPrice,
            cart.Items.Select(i => new CartItemDetails(i.ProductId, i.Quantity, i.PriceAtTimeOfAddition)).ToList()
        );
        await _eventBus.PublishAsync(integrationEvent, cancellationToken);

        await _inventoryClient.ReserveStockAsync(cart.Id, new() { { request.ProductId, request.Quantity } }, cancellationToken);

        return new CartOperationResultDto(true, null, cart.ToDto());
    }
}