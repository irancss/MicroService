using MediatR;
using DiscountService.Application.DTOs;

namespace DiscountService.Application.Features.Discounts.Queries;

/// <summary>
/// Query to get discount by ID
/// </summary>
public class GetDiscountByIdQuery : IRequest<DiscountDto?>
{
    public Guid Id { get; set; }
}

/// <summary>
/// Query to get paginated list of discounts
/// </summary>
public class GetDiscountsQuery : IRequest<DiscountListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Query to get discount usage history
/// </summary>
public class GetDiscountUsageHistoryQuery : IRequest<List<DiscountUsageHistoryDto>>
{
    public Guid DiscountId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Query to get user discount usage history
/// </summary>
public class GetUserDiscountHistoryQuery : IRequest<List<DiscountUsageHistoryDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
