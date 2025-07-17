namespace DiscountService.Application.DTOs;

/// <summary>
/// Request DTO for discount calculation
/// </summary>
public class CalculateDiscountRequest
{
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal ShippingCost { get; set; }
    public string? CouponCode { get; set; }
}

/// <summary>
/// Cart item DTO
/// </summary>
public class CartItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// Response DTO for discount calculation
/// </summary>
public class CalculateDiscountResponse
{
    public decimal OriginalTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public string DiscountDescription { get; set; } = string.Empty;
    public Guid? AppliedDiscountId { get; set; }
    public string? CouponCode { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public decimal ShippingDiscount { get; set; }
    public List<AppliedDiscountDto> AppliedDiscounts { get; set; } = new();
}

/// <summary>
/// Applied discount detail DTO
/// </summary>
public class AppliedDiscountDto
{
    public Guid DiscountId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
}
