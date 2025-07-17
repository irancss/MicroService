using DiscountService.Domain.Entities;
using DiscountService.Domain.ValueObjects;

namespace DiscountService.Domain.Services;

/// <summary>
/// Domain service for discount calculation logic
/// </summary>
public interface IDiscountCalculationService
{
    /// <summary>
    /// Calculates the best discount for a given cart
    /// </summary>
    Task<DiscountCalculationResult> CalculateBestDiscountAsync(Cart cart, List<Discount> applicableDiscounts);
    
    /// <summary>
    /// Calculates discount for a specific discount rule
    /// </summary>
    DiscountCalculationResult CalculateSpecificDiscount(Cart cart, Discount discount);
}

/// <summary>
/// Implementation of discount calculation business logic
/// </summary>
public class DiscountCalculationService : IDiscountCalculationService
{
    public async Task<DiscountCalculationResult> CalculateBestDiscountAsync(Cart cart, List<Discount> applicableDiscounts)
    {
        if (!applicableDiscounts.Any())
        {
            return DiscountCalculationResult.Success(0, cart.TotalWithShipping, "No applicable discounts");
        }

        var bestResult = DiscountCalculationResult.Success(0, cart.TotalWithShipping, "No discount applied");
        var combinableDiscounts = new List<DiscountCalculationResult>();

        foreach (var discount in applicableDiscounts.OrderByDescending(d => GetDiscountPriority(d)))
        {
            var result = CalculateSpecificDiscount(cart, discount);
            
            if (!result.IsSuccess)
                continue;

            if (discount.IsCombinableWithOthers)
            {
                combinableDiscounts.Add(result);
            }
            else if (result.DiscountAmount > bestResult.DiscountAmount)
            {
                bestResult = result;
            }
        }

        // Handle combinable discounts
        if (combinableDiscounts.Any())
        {
            var combinedResult = CombineDiscounts(cart, combinableDiscounts);
            if (combinedResult.DiscountAmount > bestResult.DiscountAmount)
            {
                bestResult = combinedResult;
            }
        }

        return await Task.FromResult(bestResult);
    }

    public DiscountCalculationResult CalculateSpecificDiscount(Cart cart, Discount discount)
    {
        try
        {
            return discount.Type switch
            {
                Domain.Enums.DiscountType.Percentage => CalculatePercentageDiscount(cart, discount),
                Domain.Enums.DiscountType.FixedAmount => CalculateFixedAmountDiscount(cart, discount),
                Domain.Enums.DiscountType.BuyXGetYFree => CalculateBuyXGetYFreeDiscount(cart, discount),
                Domain.Enums.DiscountType.FreeShipping => CalculateFreeShippingDiscount(cart, discount),
                _ => DiscountCalculationResult.Failure("Unknown discount type", cart.TotalWithShipping)
            };
        }
        catch (Exception ex)
        {
            return DiscountCalculationResult.Failure($"Error calculating discount: {ex.Message}", cart.TotalWithShipping);
        }
    }

    private DiscountCalculationResult CalculatePercentageDiscount(Cart cart, Discount discount)
    {
        var applicableAmount = GetApplicableAmount(cart, discount);
        var discountAmount = applicableAmount * (discount.Value / 100);

        if (discount.MaximumDiscountAmount.HasValue)
        {
            discountAmount = Math.Min(discountAmount, discount.MaximumDiscountAmount.Value);
        }

        var finalTotal = cart.TotalWithShipping - discountAmount;
        var description = $"{discount.Value}% off applied";

        return DiscountCalculationResult.Success(discountAmount, finalTotal, description, discount.Id, discount.CouponCode);
    }

    private DiscountCalculationResult CalculateFixedAmountDiscount(Cart cart, Discount discount)
    {
        var discountAmount = Math.Min(discount.Value, cart.SubTotal);
        var finalTotal = cart.TotalWithShipping - discountAmount;
        var description = $"{discount.Value:C} off applied";

        return DiscountCalculationResult.Success(discountAmount, finalTotal, description, discount.Id, discount.CouponCode);
    }

    private DiscountCalculationResult CalculateBuyXGetYFreeDiscount(Cart cart, Discount discount)
    {
        if (!discount.BuyQuantity.HasValue || !discount.GetQuantity.HasValue)
        {
            return DiscountCalculationResult.Failure("Invalid BOGO configuration", cart.TotalWithShipping);
        }

        var applicableItems = GetApplicableItems(cart, discount);
        var totalQuantity = applicableItems.Sum(item => item.Quantity);
        
        if (totalQuantity < discount.BuyQuantity.Value)
        {
            return DiscountCalculationResult.Failure("Not enough items for BOGO offer", cart.TotalWithShipping);
        }

        var eligibleSets = totalQuantity / discount.BuyQuantity.Value;
        var freeItems = eligibleSets * discount.GetQuantity.Value;
        
        // Calculate discount based on cheapest items that would be free
        var sortedItems = applicableItems.OrderBy(item => item.UnitPrice).ToList();
        var discountAmount = 0m;
        var remainingFreeItems = freeItems;

        foreach (var item in sortedItems)
        {
            if (remainingFreeItems <= 0) break;
            
            var freeQuantity = Math.Min(remainingFreeItems, item.Quantity);
            discountAmount += freeQuantity * item.UnitPrice;
            remainingFreeItems -= freeQuantity;
        }

        var finalTotal = cart.TotalWithShipping - discountAmount;
        var description = $"Buy {discount.BuyQuantity}, Get {discount.GetQuantity} Free - {freeItems} free items";

        return DiscountCalculationResult.Success(discountAmount, finalTotal, description, discount.Id, discount.CouponCode);
    }

    private DiscountCalculationResult CalculateFreeShippingDiscount(Cart cart, Discount discount)
    {
        var discountAmount = cart.ShippingCost;
        var finalTotal = cart.SubTotal; // Remove shipping cost
        var description = "Free shipping applied";

        return DiscountCalculationResult.Success(discountAmount, finalTotal, description, discount.Id, discount.CouponCode, cart.ShippingCost);
    }

    private decimal GetApplicableAmount(Cart cart, Discount discount)
    {
        return discount.Applicability switch
        {
            Domain.Enums.DiscountApplicability.EntireCart => cart.SubTotal,
            Domain.Enums.DiscountApplicability.SpecificProducts => GetSpecificProductsAmount(cart, discount),
            Domain.Enums.DiscountApplicability.SpecificCategories => GetSpecificCategoriesAmount(cart, discount),
            _ => 0
        };
    }

    private List<CartItem> GetApplicableItems(Cart cart, Discount discount)
    {
        return discount.Applicability switch
        {
            Domain.Enums.DiscountApplicability.EntireCart => cart.Items,
            Domain.Enums.DiscountApplicability.SpecificProducts => cart.Items.Where(item => discount.ApplicableProductIds?.Contains(item.ProductId) == true).ToList(),
            Domain.Enums.DiscountApplicability.SpecificCategories => cart.Items.Where(item => discount.ApplicableCategoryIds?.Contains(item.CategoryId) == true).ToList(),
            _ => new List<CartItem>()
        };
    }

    private decimal GetSpecificProductsAmount(Cart cart, Discount discount)
    {
        if (discount.ApplicableProductIds == null) return 0;
        
        return cart.Items
            .Where(item => discount.ApplicableProductIds.Contains(item.ProductId))
            .Sum(item => item.TotalPrice);
    }

    private decimal GetSpecificCategoriesAmount(Cart cart, Discount discount)
    {
        if (discount.ApplicableCategoryIds == null) return 0;
        
        return cart.Items
            .Where(item => discount.ApplicableCategoryIds.Contains(item.CategoryId))
            .Sum(item => item.TotalPrice);
    }

    private int GetDiscountPriority(Discount discount)
    {
        // Prioritize discounts: BOGO > FixedAmount > Percentage > FreeShipping
        return discount.Type switch
        {
            Domain.Enums.DiscountType.BuyXGetYFree => 4,
            Domain.Enums.DiscountType.FixedAmount => 3,
            Domain.Enums.DiscountType.Percentage => 2,
            Domain.Enums.DiscountType.FreeShipping => 1,
            _ => 0
        };
    }

    private DiscountCalculationResult CombineDiscounts(Cart cart, List<DiscountCalculationResult> discounts)
    {
        var totalDiscount = discounts.Sum(d => d.DiscountAmount);
        var totalShippingDiscount = discounts.Sum(d => d.ShippingDiscount);
        var finalTotal = cart.TotalWithShipping - totalDiscount;
        var description = $"Combined discounts: {string.Join(", ", discounts.Select(d => d.DiscountDescription))}";

        var result = DiscountCalculationResult.Success(totalDiscount, finalTotal, description, shippingDiscount: totalShippingDiscount);
        result.AppliedDiscounts = discounts.SelectMany(d => d.AppliedDiscounts).ToList();
        
        return result;
    }
}
