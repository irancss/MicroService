using DiscountService.Domain.Enums;

namespace DiscountService.Application.DTOs;

/// <summary>
/// DTO for creating a new discount
/// </summary>
public class CreateDiscountRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsCombinableWithOthers { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>
/// DTO for updating an existing discount
/// </summary>
public class UpdateDiscountRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsCombinableWithOthers { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? UserId { get; set; }
}

/// <summary>
/// DTO for discount response
/// </summary>
public class DiscountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CouponCode { get; set; }
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsCombinableWithOthers { get; set; }
    public int? MaxTotalUsage { get; set; }
    public int? MaxUsagePerUser { get; set; }
    public int CurrentTotalUsage { get; set; }
    public decimal? MinimumCartAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public DiscountApplicability Applicability { get; set; }
    public List<Guid>? ApplicableProductIds { get; set; }
    public List<Guid>? ApplicableCategoryIds { get; set; }
    public int? BuyQuantity { get; set; }
    public int? GetQuantity { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO for paginated discount list
/// </summary>
public class DiscountListResponse
{
    public List<DiscountDto> Discounts { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// DTO for discount usage history
/// </summary>
public class DiscountUsageHistoryDto
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
    public string DiscountName { get; set; } = string.Empty;
}
