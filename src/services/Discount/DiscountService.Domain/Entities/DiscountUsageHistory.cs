namespace DiscountService.Domain.Entities;

/// <summary>
/// Represents the usage history of a discount for auditing and tracking purposes
/// </summary>
public class DiscountUsageHistory
{
    public Guid Id { get; set; }
    public Guid DiscountId { get; set; }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal CartTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public string? CouponCode { get; set; }
    public DateTime UsedAt { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual Discount Discount { get; set; } = null!;
}
