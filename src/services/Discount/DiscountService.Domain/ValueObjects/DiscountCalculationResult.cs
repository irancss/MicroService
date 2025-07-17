namespace DiscountService.Domain.ValueObjects;

/// <summary>
/// Represents the result of a discount calculation
/// </summary>
public class DiscountCalculationResult
{
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public string DiscountDescription { get; set; } = string.Empty;
    public Guid? AppliedDiscountId { get; set; }
    public string? CouponCode { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public decimal ShippingDiscount { get; set; }
    public List<AppliedDiscountDetail> AppliedDiscounts { get; set; } = new();

    public static DiscountCalculationResult Success(decimal discountAmount, decimal finalTotal, string description, Guid? discountId = null, string? couponCode = null, decimal shippingDiscount = 0)
    {
        return new DiscountCalculationResult
        {
            DiscountAmount = discountAmount,
            FinalTotal = finalTotal,
            DiscountDescription = description,
            AppliedDiscountId = discountId,
            CouponCode = couponCode,
            IsSuccess = true,
            ShippingDiscount = shippingDiscount
        };
    }

    public static DiscountCalculationResult Failure(string errorMessage, decimal originalTotal)
    {
        return new DiscountCalculationResult
        {
            DiscountAmount = 0,
            FinalTotal = originalTotal,
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// Details of an applied discount for transparency
/// </summary>
public class AppliedDiscountDetail
{
    public Guid DiscountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
}
