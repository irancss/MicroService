using MediatR;

namespace ReportingService.Application.Queries.GetTopSellingProducts;

/// <summary>
/// Query that runs complex analytical queries on the data warehouse to find top-selling products
/// </summary>
public record GetTopSellingProductsQuery : IRequest<GetTopSellingProductsResponse>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public int TopCount { get; init; } = 10;
    public string? Category { get; init; }
    public string? Brand { get; init; }
    public string Currency { get; init; } = "USD";
    public ProductRankingBy RankBy { get; init; } = ProductRankingBy.Revenue;
}

public enum ProductRankingBy
{
    Revenue,
    Quantity,
    OrderCount
}

public record GetTopSellingProductsResponse
{
    public List<TopSellingProductItem> Products { get; init; } = new();
    public ProductAnalyticsSummary Summary { get; init; } = new();
}

public record TopSellingProductItem
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string SubCategory { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public decimal ProductPrice { get; init; }
    public decimal TotalRevenue { get; init; }
    public int TotalQuantitySold { get; init; }
    public int TotalOrders { get; init; }
    public decimal AverageOrderQuantity { get; init; }
    public decimal RevenuePercentage { get; init; }
    public int Rank { get; init; }
}

public record ProductAnalyticsSummary
{
    public decimal TotalRevenue { get; init; }
    public int TotalProductsSold { get; init; }
    public int TotalQuantitySold { get; init; }
    public int TotalOrders { get; init; }
    public int UniqueProducts { get; init; }
    public decimal TopProductsRevenuePercentage { get; init; }
}
