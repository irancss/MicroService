using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Domain.Events;
using Cart.Domain.Enums;

namespace Cart.Application.Handlers.Commands;

public class RemoveItemFromCartCommandHandler : IRequestHandler<RemoveItemFromCartCommand, CartOperationResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartConfigurationService _configService;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<RemoveItemFromCartCommandHandler> _logger;

    public RemoveItemFromCartCommandHandler(
        ICartRepository cartRepository,
        IEventPublisher eventPublisher,
        ICartConfigurationService configService,
        IInventoryGrpcClient inventoryClient,
        ILogger<RemoveItemFromCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventPublisher = eventPublisher;
        _configService = configService;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResult> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Removing item {ProductId} from {CartType} cart for user {UserId}", 
                request.ProductId, request.CartType, request.UserId);

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

            // Find and remove item from specified cart
            var items = request.CartType == CartType.Active ? cart.ActiveItems : cart.NextPurchaseItems;
            var itemToRemove = items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (itemToRemove == null)
            {
                return CartOperationResult.ErrorResult($"Item not found in {request.CartType.ToString().ToLower()} cart");
            }

            var removedQuantity = itemToRemove.Quantity;
            items.Remove(itemToRemove);
            cart.UpdateLastModified();

            // Save cart
            await _cartRepository.SaveAsync(cart);

            // Publish item removed event
            await _eventPublisher.PublishAsync(new ItemRemovedFromCartEvent
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                GuestId = cart.GuestId,
                ProductId = request.ProductId,
                RemovedQuantity = removedQuantity,
                CartType = request.CartType
            });

            // Convert to DTO
            var config = await _configService.GetConfigurationAsync();
            var cartDto = await ConvertToCartDto(cart, config);

            return CartOperationResult.SuccessResult(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
            return CartOperationResult.ErrorResult("An error occurred while removing item from cart");
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
