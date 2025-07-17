using MediatR;

namespace ReportingService.Application.Commands.AggregateSales;

/// <summary>
/// Scheduled command (via Hangfire) to pre-calculate daily sales totals
/// This improves query performance for reporting dashboards
/// </summary>
public record AggregateDailySalesCommand : IRequest<AggregateDailySalesResponse>
{
    public DateTime Date { get; init; }
    public string Currency { get; init; } = "USD";
}

public record AggregateDailySalesResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? AggregateId { get; init; }
    public decimal TotalRevenue { get; init; }
    public int TotalOrders { get; init; }
}
