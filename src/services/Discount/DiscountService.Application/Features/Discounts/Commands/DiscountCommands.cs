using MediatR;
using DiscountService.Application.DTOs;

namespace DiscountService.Application.Features.Discounts.Commands;

/// <summary>
/// Command to calculate discount for a cart
/// </summary>
public class CalculateDiscountCommand : IRequest<CalculateDiscountResponse>
{
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal ShippingCost { get; set; }
    public string? CouponCode { get; set; }
}

/// <summary>
/// Command to create a new discount
/// </summary>
public class CreateDiscountCommand : IRequest<DiscountDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public Domain.Enums.DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsCombinableWithOthers { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public Domain.Enums.DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>
/// Command to update an existing discount
/// </summary>
public class UpdateDiscountCommand : IRequest<DiscountDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public Domain.Enums.DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsCombinableWithOthers { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public Domain.Enums.DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>
/// Command to delete a discount
/// </summary>
public class DeleteDiscountCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Command to process order created event
/// </summary>
public class ProcessOrderCreatedCommand : IRequest<bool>
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
