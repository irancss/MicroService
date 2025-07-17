namespace ReportingService.Domain.DTOs;

/// <summary>
/// DTO for aggregated daily sales data calculation
/// </summary>
public class DailySalesAggregation
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItems { get; set; }
    public decimal AverageOrderValue { get; set; }
}

/// <summary>
/// DTO for product analytics data from complex queries
/// </summary>
public class ProductAnalytics
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalQuantitySold { get; set; }
    public int TotalOrders { get; set; }
}
