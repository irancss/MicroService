using BuildingBlocks.Application.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Application.Mappers;
using Cart.Domain.Entities;
using Cart.Domain.Events;

namespace Cart.Application.Handlers.Commands;

public class AddItemToNextPurchaseCartHandler : ICommandHandler<AddItemToNextPurchaseCommand, NextPurchaseCartDto>
{
    private readonly INextPurchaseCartRepository _cartRepository;
    private readonly ICatalogGrpcClient _catalogClient;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<AddItemToNextPurchaseCartHandler> _logger;

    public AddItemToNextPurchaseCartHandler(
        INextPurchaseCartRepository cartRepository,
        ICatalogGrpcClient catalogClient,
        IInventoryGrpcClient inventoryClient,
        ILogger<AddItemToNextPurchaseCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _catalogClient = catalogClient;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<NextPurchaseCartDto> Handle(AddItemToNextPurchaseCommand request, CancellationToken cancellationToken)
    {
        var productInfo = await _catalogClient.GetProductInfoAsync(request.ProductId, cancellationToken);
        if (productInfo is null || !productInfo.IsActive)
            throw new ApplicationException("Product not found or is inactive.");

        var price = await _inventoryClient.GetCurrentPriceAsync(request.ProductId, cancellationToken);
        if (price is null)
            throw new ApplicationException("Could not retrieve product price.");

        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                   ?? NextPurchaseCart.Create(request.UserId);

        var newItem = NextPurchaseItem.Create(
            productInfo.Id,
            productInfo.Name,
            price.Value,
            productInfo.ImageUrl,
            request.VariantId
        );

        cart.AddItem(newItem);

        var updatedCart = await _cartRepository.SaveAsync(cart, cancellationToken);

        _logger.LogInformation("Item {ProductId} added to next-purchase cart for user {UserId}", request.ProductId, request.UserId);

        return updatedCart.ToDto();
    }
}