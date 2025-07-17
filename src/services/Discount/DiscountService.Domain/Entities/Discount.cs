using DiscountService.Domain.Enums;

namespace DiscountService.Domain.Entities;

/// <summary>
/// Represents a discount entity with all business rules and constraints
/// </summary>
public class Discount
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; } // Null for automatic discounts
    public DiscountType Type { get; set; }
    public decimal Value { get; set; } // Percentage or fixed amount
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsAutomatic { get; set; } // True for public campaigns
    public bool IsCombinableWithOthers { get; set; }

    // Usage constraints
    public int? MaxTotalUsage { get; set; } // Global usage limit
    public int? MaxUsagePerUser { get; set; } // Per-user usage limit
    public int CurrentTotalUsage { get; set; }
    
    // Cart constraints
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; } // Cap for percentage discounts
    
    // Applicability
    public DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; } // For specific products
    public List<Guid>? ApplicableCategoryIds { get; set; } // For specific categories
    
    // BOGO specific fields
    public int? BuyQuantity { get; set; } // Buy X
    public int? GetQuantity { get; set; } // Get Y free
    
    // User-specific discount
    public Guid? UserId { get; set; } // For user-specific discounts
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<DiscountUsageHistory> UsageHistory { get; set; } = new List<DiscountUsageHistory>();

    /// <summary>
    /// Checks if the discount is currently valid based on dates and active status
    /// </summary>
    public bool IsCurrentlyValid()
    {
        var now = DateTime.UtcNow;
        return IsActive && StartDate <= now && EndDate >= now;
    }

    /// <summary>
    /// Checks if the discount has reached its usage limit
    /// </summary>
    public bool HasReachedUsageLimit()
    {
        return MaxTotalUsage.HasValue && CurrentTotalUsage >= MaxTotalUsage.Value;
    }

    /// <summary>
    /// Checks if a user has reached their usage limit for this discount
    /// </summary>
    public bool HasUserReachedUsageLimit(Guid userId, int userUsageCount)
    {
        return MaxUsagePerUser.HasValue && userUsageCount >= MaxUsagePerUser.Value;
    }

    /// <summary>
    /// Validates if the discount can be applied to a cart
    /// </summary>
    public bool CanBeAppliedToCart(decimal cartTotal)
    {
        return !MinimumCartAmount.HasValue || cartTotal >= MinimumCartAmount.Value;
    }
}
