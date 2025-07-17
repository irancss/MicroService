using MediatR;

namespace ReportingService.Application.Queries.GetDailySalesReport;

/// <summary>
/// Query to get pre-aggregated daily sales data for fast reporting
/// </summary>
public record GetDailySalesReportQuery : IRequest<GetDailySalesReportResponse>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string Currency { get; init; } = "USD";
}

public record GetDailySalesReportResponse
{
    public List<DailySalesReportItem> DailySales { get; init; } = new();
    public DailySalesSummary Summary { get; init; } = new();
}

public record DailySalesReportItem
{
    public DateTime Date { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TotalTax { get; init; }
    public decimal TotalDiscount { get; init; }
    public int TotalOrders { get; init; }
    public int TotalItems { get; init; }
    public decimal AverageOrderValue { get; init; }
    public string Currency { get; init; } = string.Empty;
}

public record DailySalesSummary
{
    public decimal TotalRevenue { get; init; }
    public decimal TotalTax { get; init; }
    public decimal TotalDiscount { get; init; }
    public int TotalOrders { get; init; }
    public int TotalItems { get; init; }
    public decimal AverageOrderValue { get; init; }
    public decimal DailyAverageRevenue { get; init; }
    public int NumberOfDays { get; init; }
}
