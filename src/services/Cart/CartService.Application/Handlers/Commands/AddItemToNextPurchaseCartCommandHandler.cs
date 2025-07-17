using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Domain.Entities;
using Cart.Domain.Events;
using Cart.Domain.Enums;

namespace Cart.Application.Handlers.Commands;

public class AddItemToNextPurchaseCartCommandHandler : IRequestHandler<AddItemToNextPurchaseCartCommand, CartOperationResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ICatalogGrpcClient _catalogClient;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartConfigurationService _configService;
    private readonly ILogger<AddItemToNextPurchaseCartCommandHandler> _logger;

    public AddItemToNextPurchaseCartCommandHandler(
        ICartRepository cartRepository,
        IInventoryGrpcClient inventoryClient,
        ICatalogGrpcClient catalogClient,
        IEventPublisher eventPublisher,
        ICartConfigurationService configService,
        ILogger<AddItemToNextPurchaseCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _inventoryClient = inventoryClient;
        _catalogClient = catalogClient;
        _eventPublisher = eventPublisher;
        _configService = configService;
        _logger = logger;
    }

    public async Task<CartOperationResult> Handle(AddItemToNextPurchaseCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding item {ProductId} to next purchase cart for user {UserId}", request.ProductId, request.UserId);

            // Validate product exists and get info
            var productInfo = await _catalogClient.GetProductInfoAsync(request.ProductId);
            if (productInfo == null || !productInfo.IsActive)
            {
                return CartOperationResult.ErrorResult("Product not found or inactive");
            }

            // Get current price
            var config = await _configService.GetConfigurationAsync();
            var currentPrice = config.RealTimePriceValidationEnabled
                ? await _inventoryClient.GetCurrentPriceAsync(request.ProductId)
                : null;

            if (currentPrice == null)
            {
                return CartOperationResult.ErrorResult("Unable to retrieve product price");
            }

            // Get or create cart
            var cart = await GetOrCreateCartAsync(request.UserId, request.GuestId);

            // Add or update item in next purchase cart
            var existingItem = cart.NextPurchaseItems.FirstOrDefault(i => i.ProductId == request.ProductId && i.VariantId == request.VariantId);
            
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + request.Quantity);
                existingItem.UpdatePrice(currentPrice.Value);
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = request.ProductId,
                    ProductName = productInfo.Name,
                    ProductImageUrl = productInfo.ImageUrl,
                    Quantity = request.Quantity,
                    PriceAtTimeOfAddition = currentPrice.Value,
                    AddedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    VariantId = request.VariantId,
                    Attributes = request.Attributes
                };
                cart.NextPurchaseItems.Add(newItem);
            }

            cart.UpdateLastModified();

            // Save cart
            await _cartRepository.SaveAsync(cart);

            // Publish item added event
            await _eventPublisher.PublishAsync(new ItemAddedToCartEvent
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                GuestId = cart.GuestId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = currentPrice.Value,
                CartType = CartType.NextPurchase
            });

            // Convert to DTO
            var cartDto = await ConvertToCartDto(cart, config);
            
            return CartOperationResult.SuccessResult(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to next purchase cart");
            return CartOperationResult.ErrorResult("An error occurred while adding item to next purchase cart");
        }
    }

    private async Task<ShoppingCart> GetOrCreateCartAsync(string? userId, string? guestId)
    {
        ShoppingCart? cart = null;

        if (!string.IsNullOrEmpty(userId))
        {
            cart = await _cartRepository.GetByUserIdAsync(userId);
        }
        else if (!string.IsNullOrEmpty(guestId))
        {
            cart = await _cartRepository.GetByGuestIdAsync(guestId);
        }

        if (cart == null)
        {
            cart = new ShoppingCart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                GuestId = guestId,
                CreatedUtc = DateTime.UtcNow,
                LastModifiedUtc = DateTime.UtcNow
            };
        }

        return cart;
    }

    private async Task<CartDto> ConvertToCartDto(ShoppingCart cart, Domain.ValueObjects.CartConfiguration config)
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

    private async Task<List<CartItemDto>> ConvertCartItemsToDto(List<CartItem> items, Domain.ValueObjects.CartConfiguration config)
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
