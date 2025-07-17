namespace DiscountService.Domain.Events;

/// <summary>
/// Event published when an order is created and a discount was applied
/// </summary>
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal CartTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public Guid? DiscountId { get; set; }
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime OrderCreatedAt { get; set; }
}
