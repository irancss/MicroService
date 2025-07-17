using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Domain.Events;
using Cart.Domain.Enums;

namespace Cart.Application.Handlers.Commands;

public class MoveItemToActiveCartCommandHandler : IRequestHandler<MoveItemToActiveCartCommand, CartOperationResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartConfigurationService _configService;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<MoveItemToActiveCartCommandHandler> _logger;

    public MoveItemToActiveCartCommandHandler(
        ICartRepository cartRepository,
        IEventPublisher eventPublisher,
        ICartConfigurationService configService,
        IInventoryGrpcClient inventoryClient,
        ILogger<MoveItemToActiveCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventPublisher = eventPublisher;
        _configService = configService;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResult> Handle(MoveItemToActiveCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Moving item {ProductId} to active cart for user {UserId}", request.ProductId, request.UserId);

            // Get cart
            Domain.Entities.ShoppingCart? cart = null;
            if (!string.IsNullOrEmpty(request.UserId))
            {
                cart = await _cartRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.GuestId))
            {
                cart = await _cartRepository.GetByGuestIdAsync(request.GuestId);
            }

            if (cart == null)
            {
                return CartOperationResult.ErrorResult("Cart not found");
            }

            // Find item in next purchase cart
            var nextPurchaseItem = cart.NextPurchaseItems.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (nextPurchaseItem == null)
            {
                return CartOperationResult.ErrorResult("Item not found in next purchase cart");
            }

            var quantityToMove = request.Quantity ?? nextPurchaseItem.Quantity;
            if (quantityToMove > nextPurchaseItem.Quantity)
            {
                return CartOperationResult.ErrorResult("Cannot move more items than available");
            }

            // Check if item already exists in active cart
            var activeItem = cart.ActiveItems.FirstOrDefault(i => i.ProductId == request.ProductId && i.VariantId == nextPurchaseItem.VariantId);

            if (activeItem != null)
            {
                // Update existing item
                activeItem.UpdateQuantity(activeItem.Quantity + quantityToMove);
                activeItem.UpdatePrice(nextPurchaseItem.PriceAtTimeOfAddition);
            }
            else
            {
                // Create new item in active cart
                var newItem = nextPurchaseItem.Clone();
                newItem.Quantity = quantityToMove;
                newItem.LastUpdatedUtc = DateTime.UtcNow;
                cart.ActiveItems.Add(newItem);
            }

            // Update or remove from next purchase cart
            if (quantityToMove == nextPurchaseItem.Quantity)
            {
                cart.NextPurchaseItems.Remove(nextPurchaseItem);
            }
            else
            {
                nextPurchaseItem.UpdateQuantity(nextPurchaseItem.Quantity - quantityToMove);
            }

            cart.UpdateLastModified();

            // Save cart
            await _cartRepository.SaveAsync(cart);

            // Publish event
            await _eventPublisher.PublishAsync(new ItemMovedBetweenCartsEvent
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                GuestId = cart.GuestId,
                ProductId = request.ProductId,
                Quantity = quantityToMove,
                SourceCartType = CartType.NextPurchase,
                DestinationCartType = CartType.Active
            });

            // Convert to DTO
            var config = await _configService.GetConfigurationAsync();
            var cartDto = await ConvertToCartDto(cart, config);

            return CartOperationResult.SuccessResult(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving item to active cart");
            return CartOperationResult.ErrorResult("An error occurred while moving item");
        }
    }

    private async Task<CartDto> ConvertToCartDto(Domain.Entities.ShoppingCart cart, Domain.ValueObjects.CartConfiguration config)
    {
        var cartDto = new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            GuestId = cart.GuestId,
            CreatedUtc = cart.CreatedUtc,
            LastModifiedUtc = cart.LastModifiedUtc,
            ActiveTotalAmount = cart.GetActiveTotalAmount(),
            NextPurchaseTotalAmount = cart.GetNextPurchaseTotalAmount(),
            ActiveItemsCount = cart.GetActiveItemsCount(),
            NextPurchaseItemsCount = cart.GetNextPurchaseItemsCount()
        };

        // Convert active items
        cartDto.ActiveItems = await ConvertCartItemsToDto(cart.ActiveItems, config);
        
        // Convert next purchase items
        cartDto.NextPurchaseItems = await ConvertCartItemsToDto(cart.NextPurchaseItems, config);

        return cartDto;
    }

    private async Task<List<CartItemDto>> ConvertCartItemsToDto(List<Domain.Entities.CartItem> items, Domain.ValueObjects.CartConfiguration config)
    {
        var result = new List<CartItemDto>();
        
        if (!items.Any()) return result;

        // Get current prices and stock status if validation is enabled
        Dictionary<string, decimal>? currentPrices = null;
        Dictionary<string, bool>? stockStatus = null;

        if (config.RealTimePriceValidationEnabled)
        {
            currentPrices = await _inventoryClient.GetMultiplePricesAsync(items.Select(i => i.ProductId).ToList());
        }

        if (config.RealTimeStockValidationEnabled)
        {
            var stockQueries = items.ToDictionary(i => i.ProductId, i => i.Quantity);
            stockStatus = await _inventoryClient.CheckMultipleStockAvailabilityAsync(stockQueries);
        }

        foreach (var item in items)
        {
            var currentPrice = currentPrices?.GetValueOrDefault(item.ProductId, item.PriceAtTimeOfAddition) ?? item.PriceAtTimeOfAddition;
            var inStock = stockStatus?.GetValueOrDefault(item.ProductId, true) ?? true;

            result.Add(new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImageUrl = item.ProductImageUrl,
                Quantity = item.Quantity,
                PriceAtTimeOfAddition = item.PriceAtTimeOfAddition,
                CurrentPrice = currentPrice,
                PriceChanged = Math.Abs(currentPrice - item.PriceAtTimeOfAddition) > 0.01m,
                InStock = inStock,
                AddedUtc = item.AddedUtc,
                LastUpdatedUtc = item.LastUpdatedUtc,
                VariantId = item.VariantId,
                Attributes = item.Attributes
            });
        }

        return result;
    }
}
