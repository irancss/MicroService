using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Domain.Events;
using Cart.Domain.Enums;

namespace Cart.Application.Handlers.Commands;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, CartOperationResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartConfigurationService _configService;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<UpdateCartItemQuantityCommandHandler> _logger;

    public UpdateCartItemQuantityCommandHandler(
        ICartRepository cartRepository,
        IEventPublisher eventPublisher,
        ICartConfigurationService configService,
        IInventoryGrpcClient inventoryClient,
        ILogger<UpdateCartItemQuantityCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventPublisher = eventPublisher;
        _configService = configService;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResult> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating item {ProductId} quantity to {Quantity} in {CartType} cart", 
                request.ProductId, request.Quantity, request.CartType);

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

            // Find item in specified cart
            var items = request.CartType == CartType.Active ? cart.ActiveItems : cart.NextPurchaseItems;
            var itemToUpdate = items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (itemToUpdate == null)
            {
                return CartOperationResult.ErrorResult($"Item not found in {request.CartType.ToString().ToLower()} cart");
            }

            // Check stock if updating active cart
            var config = await _configService.GetConfigurationAsync();
            if (request.CartType == CartType.Active && config.RealTimeStockValidationEnabled)
            {
                var isInStock = await _inventoryClient.CheckStockAvailabilityAsync(request.ProductId, request.Quantity);
                if (!isInStock)
                {
                    return CartOperationResult.ErrorResult("Insufficient stock for requested quantity");
                }
            }

            var oldQuantity = itemToUpdate.Quantity;
            itemToUpdate.UpdateQuantity(request.Quantity);
            cart.UpdateLastModified();

            // Save cart
            await _cartRepository.SaveAsync(cart);

            // Publish event (treating as item added/removed based on quantity change)
            if (request.Quantity > oldQuantity)
            {
                await _eventPublisher.PublishAsync(new ItemAddedToCartEvent
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    GuestId = cart.GuestId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity - oldQuantity,
                    Price = itemToUpdate.PriceAtTimeOfAddition,
                    CartType = request.CartType
                });
            }
            else if (request.Quantity < oldQuantity)
            {
                await _eventPublisher.PublishAsync(new ItemRemovedFromCartEvent
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    GuestId = cart.GuestId,
                    ProductId = request.ProductId,
                    RemovedQuantity = oldQuantity - request.Quantity,
                    CartType = request.CartType
                });
            }

            // Convert to DTO
            var cartDto = await ConvertToCartDto(cart, config);

            return CartOperationResult.SuccessResult(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item quantity");
            return CartOperationResult.ErrorResult("An error occurred while updating item quantity");
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

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, CartOperationResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICartConfigurationService _configService;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<ClearCartCommandHandler> _logger;

    public ClearCartCommandHandler(
        ICartRepository cartRepository,
        IEventPublisher eventPublisher,
        ICartConfigurationService configService,
        IInventoryGrpcClient inventoryClient,
        ILogger<ClearCartCommandHandler> logger)
    {
        _cartRepository = cartRepository;
        _eventPublisher = eventPublisher;
        _configService = configService;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartOperationResult> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Clearing {CartType} cart for user {UserId}", request.CartType, request.UserId);

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

            // Clear specified cart
            if (request.CartType == CartType.Active)
            {
                cart.ActiveItems.Clear();
            }
            else
            {
                cart.NextPurchaseItems.Clear();
            }

            cart.UpdateLastModified();

            // Save cart
            await _cartRepository.SaveAsync(cart);

            // Publish event
            await _eventPublisher.PublishAsync(new Domain.Events.CartClearedEvent
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                GuestId = cart.GuestId,
                EventType = Domain.Enums.CartEventType.CartCleared,
            });

            // Convert to DTO
            var config = await _configService.GetConfigurationAsync();
            var cartDto = await ConvertToCartDto(cart, config);

            return CartOperationResult.SuccessResult(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return CartOperationResult.ErrorResult("An error occurred while clearing cart");
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
