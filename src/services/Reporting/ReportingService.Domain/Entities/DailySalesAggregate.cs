namespace ReportingService.Domain.Entities;

/// <summary>
/// Pre-aggregated daily sales data for fast reporting queries
/// </summary>
public class DailySalesAggregate : BaseEntity
{
    public DateTime Date { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public decimal TotalTax { get; private set; }
    public decimal TotalDiscount { get; private set; }
    public int TotalOrders { get; private set; }
    public int TotalItems { get; private set; }
    public decimal AverageOrderValue { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    private DailySalesAggregate() { } // EF Core

    public DailySalesAggregate(
        DateTime date, 
        decimal totalRevenue, 
        decimal totalTax, 
        decimal totalDiscount,
        int totalOrders, 
        int totalItems, 
        decimal averageOrderValue, 
        string currency)
    {
        Date = date.Date; // Ensure only date part
        TotalRevenue = totalRevenue;
        TotalTax = totalTax;
        TotalDiscount = totalDiscount;
        TotalOrders = totalOrders;
        TotalItems = totalItems;
        AverageOrderValue = averageOrderValue;
        Currency = currency;
    }

    public void UpdateAggregates(
        decimal totalRevenue, 
        decimal totalTax, 
        decimal totalDiscount,
        int totalOrders, 
        int totalItems, 
        decimal averageOrderValue)
    {
        TotalRevenue = totalRevenue;
        TotalTax = totalTax;
        TotalDiscount = totalDiscount;
        TotalOrders = totalOrders;
        TotalItems = totalItems;
        AverageOrderValue = averageOrderValue;
        SetUpdatedAt();
    }
}
