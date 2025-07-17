using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Queries.GetDailySalesReport;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesReportsController> _logger;

    public SalesReportsController(IMediator mediator, ILogger<SalesReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get daily sales report for a specified date range
    /// </summary>
    /// <param name="fromDate">Start date (YYYY-MM-DD)</param>
    /// <param name="toDate">End date (YYYY-MM-DD)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <returns>Daily sales report with aggregated data</returns>
    [HttpGet("daily")]
    public async Task<ActionResult<GetDailySalesReportResponse>> GetDailySalesReport(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string currency = "USD")
    {
        try
        {
            var query = new GetDailySalesReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving daily sales report");
            return StatusCode(500, "An error occurred while retrieving the sales report");
        }
    }

    /// <summary>
    /// Get daily sales report for last 30 days
    /// </summary>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <returns>Daily sales report for last 30 days</returns>
    [HttpGet("daily/last30days")]
    public async Task<ActionResult<GetDailySalesReportResponse>> GetLast30DaysSalesReport(
        [FromQuery] string currency = "USD")
    {
        try
        {
            var toDate = DateTime.Today;
            var fromDate = toDate.AddDays(-30);

            var query = new GetDailySalesReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving last 30 days sales report");
            return StatusCode(500, "An error occurred while retrieving the sales report");
        }
    }

    /// <summary>
    /// Get monthly sales summary for the current year
    /// </summary>
    /// <param name="year">Year (default: current year)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <returns>Monthly sales summary</returns>
    [HttpGet("monthly/{year:int?}")]
    public async Task<ActionResult<GetDailySalesReportResponse>> GetMonthlySalesReport(
        int? year = null,
        [FromQuery] string currency = "USD")
    {
        try
        {
            var targetYear = year ?? DateTime.Now.Year;
            var fromDate = new DateTime(targetYear, 1, 1);
            var toDate = new DateTime(targetYear, 12, 31);

            var query = new GetDailySalesReportQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                Currency = currency
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly sales report for year {Year}", year);
            return StatusCode(500, "An error occurred while retrieving the sales report");
        }
    }
}
