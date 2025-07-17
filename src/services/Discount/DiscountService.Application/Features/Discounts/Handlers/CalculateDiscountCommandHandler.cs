using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using DiscountService.Application.DTOs;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Application.Interfaces;
using DiscountService.Domain.Entities;
using DiscountService.Domain.Services;
using DiscountService.Domain.ValueObjects;

namespace DiscountService.Application.Features.Discounts.Handlers;

/// <summary>
/// Handler for calculating discount
/// </summary>
public class CalculateDiscountCommandHandler : IRequestHandler<CalculateDiscountCommand, CalculateDiscountResponse>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IDiscountUsageHistoryRepository _usageHistoryRepository;
    private readonly IDiscountCalculationService _calculationService;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CalculateDiscountCommandHandler> _logger;

    public CalculateDiscountCommandHandler(
        IDiscountRepository discountRepository,
        IDiscountUsageHistoryRepository usageHistoryRepository,
        IDiscountCalculationService calculationService,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<CalculateDiscountCommandHandler> logger)
    {
        _discountRepository = discountRepository;
        _usageHistoryRepository = usageHistoryRepository;
        _calculationService = calculationService;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CalculateDiscountResponse> Handle(CalculateDiscountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Calculating discount for user {UserId} with coupon {CouponCode}", 
                request.UserId, request.CouponCode);

            // Convert request to domain object
            var cart = new Cart
            {
                UserId = request.UserId,
                Items = request.Items.Select(item => new CartItem(
                    item.ProductId, item.ProductName, item.CategoryId, 
                    item.CategoryName, item.UnitPrice, item.Quantity)).ToList(),
                ShippingCost = request.ShippingCost,
                CouponCode = request.CouponCode
            };

            // Get applicable discounts
            var applicableDiscounts = await GetApplicableDiscountsAsync(cart);

            // Calculate best discount
            var result = await _calculationService.CalculateBestDiscountAsync(cart, applicableDiscounts);

            // Map to response
            var response = new CalculateDiscountResponse
            {
                OriginalTotal = cart.TotalWithShipping,
                DiscountAmount = result.DiscountAmount,
                FinalTotal = result.FinalTotal,
                DiscountDescription = result.DiscountDescription,
                AppliedDiscountId = result.AppliedDiscountId,
                CouponCode = result.CouponCode,
                IsSuccess = result.IsSuccess,
                ErrorMessage = result.ErrorMessage,
                ShippingDiscount = result.ShippingDiscount,
                AppliedDiscounts = result.AppliedDiscounts.Select(d => new AppliedDiscountDto
                {
                    DiscountId = d.DiscountId,
                    Name = d.Name,
                    Amount = d.Amount,
                    Description = d.Description,
                    CouponCode = d.CouponCode
                }).ToList()
            };

            _logger.LogInformation("Discount calculation completed. Amount: {Amount}, Final Total: {FinalTotal}", 
                response.DiscountAmount, response.FinalTotal);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating discount for user {UserId}", request.UserId);
            return new CalculateDiscountResponse
            {
                OriginalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity) + request.ShippingCost,
                DiscountAmount = 0,
                FinalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity) + request.ShippingCost,
                IsSuccess = false,
                ErrorMessage = "An error occurred while calculating discount"
            };
        }
    }

    private async Task<List<Discount>> GetApplicableDiscountsAsync(Cart cart)
    {
        var applicableDiscounts = new List<Discount>();

        // Check cache first for automatic discounts
        var cacheKey = "automatic_discounts";
        var cachedDiscounts = await _cacheService.GetAsync<List<Discount>>(cacheKey);
        
        List<Discount> automaticDiscounts;
        if (cachedDiscounts != null)
        {
            automaticDiscounts = cachedDiscounts;
        }
        else
        {
            automaticDiscounts = await _discountRepository.GetActiveAutomaticDiscountsAsync();
            await _cacheService.SetAsync(cacheKey, automaticDiscounts, TimeSpan.FromMinutes(5));
        }

        // Add valid automatic discounts
        foreach (var discount in automaticDiscounts)
        {
            if (IsDiscountApplicable(discount, cart))
            {
                applicableDiscounts.Add(discount);
            }
        }

        // Check for coupon code
        if (!string.IsNullOrEmpty(cart.CouponCode))
        {
            var couponCacheKey = $"coupon_{cart.CouponCode}";
            var cachedCoupon = await _cacheService.GetAsync<Discount>(couponCacheKey);
            
            Discount? couponDiscount;
            if (cachedCoupon != null)
            {
                couponDiscount = cachedCoupon;
            }
            else
            {
                couponDiscount = await _discountRepository.GetByCouponCodeAsync(cart.CouponCode);
                if (couponDiscount != null)
                {
                    await _cacheService.SetAsync(couponCacheKey, couponDiscount, TimeSpan.FromMinutes(10));
                }
            }

            if (couponDiscount != null && IsDiscountApplicable(couponDiscount, cart))
            {
                applicableDiscounts.Add(couponDiscount);
            }
        }

        // Check for user-specific discounts
        var userDiscounts = await _discountRepository.GetUserSpecificDiscountsAsync(cart.UserId);
        foreach (var discount in userDiscounts)
        {
            if (IsDiscountApplicable(discount, cart))
            {
                applicableDiscounts.Add(discount);
            }
        }

        return applicableDiscounts.Distinct().ToList();
    }

    private bool IsDiscountApplicable(Discount discount, Cart cart)
    {
        // Check basic validity
        if (!discount.IsCurrentlyValid())
            return false;

        // Check usage limits
        if (discount.HasReachedUsageLimit())
            return false;

        // Check minimum cart amount
        if (!discount.CanBeAppliedToCart(cart.SubTotal))
            return false;

        // Check user-specific constraints
        if (discount.UserId.HasValue && discount.UserId.Value != cart.UserId)
            return false;

        return true;
    }
}
