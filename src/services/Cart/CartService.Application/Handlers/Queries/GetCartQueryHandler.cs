using MediatR;
using Microsoft.Extensions.Logging;
using Cart.Application.Queries;
using Cart.Application.DTOs;
using Cart.Application.Interfaces;
using Cart.Domain.Entities;

namespace Cart.Application.Handlers.Queries;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto?>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartConfigurationService _configService;
    private readonly IInventoryGrpcClient _inventoryClient;
    private readonly ILogger<GetCartQueryHandler> _logger;

    public GetCartQueryHandler(
        ICartRepository cartRepository,
        ICartConfigurationService configService,
        IInventoryGrpcClient inventoryClient,
        ILogger<GetCartQueryHandler> logger)
    {
        _cartRepository = cartRepository;
        _configService = configService;
        _inventoryClient = inventoryClient;
        _logger = logger;
    }

    public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting cart for user {UserId} or guest {GuestId}", request.UserId, request.GuestId);

            ShoppingCart? cart = null;

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
                return null;
            }

            // Convert to DTO
            var config = await _configService.GetConfigurationAsync();
            var cartDto = await ConvertToCartDto(cart, config, request.ValidateStockAndPrices);

            return cartDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return null;
        }
    }

    private async Task<CartDto> ConvertToCartDto(ShoppingCart cart, Domain.ValueObjects.CartConfiguration config, bool validateStockAndPrices)
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
        cartDto.ActiveItems = await ConvertCartItemsToDto(cart.ActiveItems, config, validateStockAndPrices);
        
        // Convert next purchase items
        cartDto.NextPurchaseItems = await ConvertCartItemsToDto(cart.NextPurchaseItems, config, validateStockAndPrices);

        return cartDto;
    }

    private async Task<List<CartItemDto>> ConvertCartItemsToDto(List<CartItem> items, Domain.ValueObjects.CartConfiguration config, bool validateStockAndPrices)
    {
        var result = new List<CartItemDto>();
        
        if (!items.Any()) return result;

        // Get current prices and stock status if validation is enabled
        Dictionary<string, decimal>? currentPrices = null;
        Dictionary<string, bool>? stockStatus = null;

        if (validateStockAndPrices && config.RealTimePriceValidationEnabled)
        {
            currentPrices = await _inventoryClient.GetMultiplePricesAsync(items.Select(i => i.ProductId).ToList());
        }

        if (validateStockAndPrices && config.RealTimeStockValidationEnabled)
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

public class GetCartSummaryQueryHandler : IRequestHandler<GetCartSummaryQuery, CartSummaryDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly ILogger<GetCartSummaryQueryHandler> _logger;

    public GetCartSummaryQueryHandler(
        ICartRepository cartRepository,
        ILogger<GetCartSummaryQueryHandler> logger)
    {
        _cartRepository = cartRepository;
        _logger = logger;
    }

    public async Task<CartSummaryDto> Handle(GetCartSummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            ShoppingCart? cart = null;

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
                return new CartSummaryDto
                {
                    HasActiveItems = false,
                    HasNextPurchaseItems = false,
                    ActiveItemsCount = 0,
                    NextPurchaseItemsCount = 0,
                    ActiveTotalAmount = 0,
                    NextPurchaseTotalAmount = 0
                };
            }

            return new CartSummaryDto
            {
                HasActiveItems = cart.HasActiveItems(),
                HasNextPurchaseItems = cart.HasNextPurchaseItems(),
                ActiveItemsCount = cart.GetActiveItemsCount(),
                NextPurchaseItemsCount = cart.GetNextPurchaseItemsCount(),
                ActiveTotalAmount = cart.GetActiveTotalAmount(),
                NextPurchaseTotalAmount = cart.GetNextPurchaseTotalAmount(),
                LastModifiedUtc = cart.LastModifiedUtc
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart summary");
            return new CartSummaryDto();
        }
    }
}
