using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Interfaces;

namespace ReportingService.Application.Queries.GetDailySalesReport;

public class GetDailySalesReportHandler : IRequestHandler<GetDailySalesReportQuery, GetDailySalesReportResponse>
{
    private readonly IReportingRepository _repository;
    private readonly ILogger<GetDailySalesReportHandler> _logger;

    public GetDailySalesReportHandler(
        IReportingRepository repository,
        ILogger<GetDailySalesReportHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<GetDailySalesReportResponse> Handle(
        GetDailySalesReportQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting daily sales report from {FromDate} to {ToDate} for currency {Currency}", 
                request.FromDate, request.ToDate, request.Currency);

            // Get pre-aggregated daily sales data
            var dailySalesAggregates = await _repository.GetDailySalesAggregatesRangeAsync(
                request.FromDate, 
                request.ToDate, 
                request.Currency, 
                cancellationToken);

            var dailySalesItems = dailySalesAggregates.Select(x => new DailySalesReportItem
            {
                Date = x.Date,
                TotalRevenue = x.TotalRevenue,
                TotalTax = x.TotalTax,
                TotalDiscount = x.TotalDiscount,
                TotalOrders = x.TotalOrders,
                TotalItems = x.TotalItems,
                AverageOrderValue = x.AverageOrderValue,
                Currency = x.Currency
            }).OrderBy(x => x.Date).ToList();

            // Calculate summary
            var summary = CalculateSummary(dailySalesItems, request.FromDate, request.ToDate);

            _logger.LogInformation("Retrieved {Count} daily sales records", dailySalesItems.Count);

            return new GetDailySalesReportResponse
            {
                DailySales = dailySalesItems,
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting daily sales report");
            throw;
        }
    }

    private static DailySalesSummary CalculateSummary(List<DailySalesReportItem> dailySales, DateTime fromDate, DateTime toDate)
    {
        if (!dailySales.Any())
        {
            return new DailySalesSummary
            {
                NumberOfDays = (int)(toDate - fromDate).TotalDays + 1
            };
        }

        var totalRevenue = dailySales.Sum(x => x.TotalRevenue);
        var totalTax = dailySales.Sum(x => x.TotalTax);
        var totalDiscount = dailySales.Sum(x => x.TotalDiscount);
        var totalOrders = dailySales.Sum(x => x.TotalOrders);
        var totalItems = dailySales.Sum(x => x.TotalItems);
        var numberOfDays = (int)(toDate - fromDate).TotalDays + 1;
        var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var dailyAverageRevenue = numberOfDays > 0 ? totalRevenue / numberOfDays : 0;

        return new DailySalesSummary
        {
            TotalRevenue = totalRevenue,
            TotalTax = totalTax,
            TotalDiscount = totalDiscount,
            TotalOrders = totalOrders,
            TotalItems = totalItems,
            AverageOrderValue = averageOrderValue,
            DailyAverageRevenue = dailyAverageRevenue,
            NumberOfDays = numberOfDays
        };
    }
}
