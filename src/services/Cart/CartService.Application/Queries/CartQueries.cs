using BuildingBlocks.Application.Abstractions;
using MediatR;
using Cart.Application.DTOs;

namespace Cart.Application.Queries;


public record GetCartByUserIdQuery(string UserId) : IQuery<CartDto?>;

public record GetActiveCartQuery(string CartId) : IQuery<CartDto?>;
public record GetNextPurchaseCartQuery(string UserId) : IQuery<NextPurchaseCartDto>;

public class GetCartQuery : IRequest<CartDto?>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public bool ValidateStockAndPrices { get; set; } = true;
}

public class GetCartByIdQuery : IRequest<CartDto?>
{
    public string CartId { get; set; } = string.Empty;
    public bool ValidateStockAndPrices { get; set; } = true;
}

public class GetAbandonedCartsQuery : IRequest<List<CartDto>>
{
    public DateTime? SinceDate { get; set; }
    public int MaxResults { get; set; } = 100;
}

public class GetCartSummaryQuery : IRequest<CartSummaryDto>
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
}

public class CartSummaryDto
{
    public bool HasActiveItems { get; set; }
    public bool HasNextPurchaseItems { get; set; }
    public int ActiveItemsCount { get; set; }
    public int NextPurchaseItemsCount { get; set; }
    public decimal ActiveTotalAmount { get; set; }
    public decimal NextPurchaseTotalAmount { get; set; }
    public DateTime? LastModifiedUtc { get; set; }
}
