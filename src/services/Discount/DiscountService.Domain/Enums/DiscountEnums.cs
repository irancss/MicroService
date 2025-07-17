namespace DiscountService.Domain.Enums;

/// <summary>
/// Defines the different types of discounts supported by the system
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// Percentage discount (e.g., 20% off)
    /// </summary>
    Percentage = 1,
    
    /// <summary>
    /// Fixed amount discount (e.g., 50,000 Toman off)
    /// </summary>
    FixedAmount = 2,
    
    /// <summary>
    /// Buy X, Get Y Free discount
    /// </summary>
    BuyXGetYFree = 3,
    
    /// <summary>
    /// Free shipping discount
    /// </summary>
    FreeShipping = 4
}

/// <summary>
/// Defines where the discount can be applied
/// </summary>
public enum DiscountApplicability
{
    /// <summary>
    /// Applied to the entire cart
    /// </summary>
    EntireCart = 1,
    
    /// <summary>
    /// Applied only to specific products
    /// </summary>
    SpecificProducts = 2,
    
    /// <summary>
    /// Applied only to specific categories
    /// </summary>
    SpecificCategories = 3
}
