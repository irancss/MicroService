using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Commands.AggregateSales;
using ReportingService.Infrastructure.BackgroundJobs;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataManagementController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DataManagementController> _logger;

    public DataManagementController(IMediator mediator, ILogger<DataManagementController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Manually trigger daily sales aggregation for a specific date
    /// </summary>
    /// <param name="date">Date to aggregate (YYYY-MM-DD)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <returns>Aggregation result</returns>
    [HttpPost("aggregate/daily/{date:datetime}")]
    public async Task<ActionResult> TriggerDailySalesAggregation(
        DateTime date,
        [FromQuery] string currency = "USD")
    {
        try
        {
            _logger.LogInformation("Manual trigger for daily sales aggregation for date: {Date}", date);

            var command = new AggregateDailySalesCommand
            {
                Date = date,
                Currency = currency
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Message = result.Message,
                    Data = new
                    {
                        Date = date,
                        Currency = currency,
                        TotalRevenue = result.TotalRevenue,
                        TotalOrders = result.TotalOrders,
                        AggregateId = result.AggregateId
                    }
                });
            }
            else
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering daily sales aggregation for date: {Date}", date);
            return StatusCode(500, "An error occurred while processing the aggregation");
        }
    }

    /// <summary>
    /// Trigger historical data re-aggregation for a date range
    /// </summary>
    /// <param name="fromDate">Start date (YYYY-MM-DD)</param>
    /// <param name="toDate">End date (YYYY-MM-DD)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <returns>Background job ID</returns>
    [HttpPost("aggregate/historical")]
    public ActionResult TriggerHistoricalReaggregation(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string currency = "USD")
    {
        try
        {
            if ((toDate - fromDate).TotalDays > 365)
            {
                return BadRequest("Date range cannot exceed 365 days");
            }

            _logger.LogInformation("Triggering historical re-aggregation from {FromDate} to {ToDate}", fromDate, toDate);

            // Schedule as background job with Hangfire
            var jobId = BackgroundJob.Enqueue<SalesAggregationJobs>(
                x => x.ReaggregateHistoricalData(fromDate, toDate, currency));

            return Ok(new
            {
                Success = true,
                Message = "Historical re-aggregation job has been queued",
                JobId = jobId,
                Data = new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Currency = currency,
                    EstimatedDays = (int)(toDate - fromDate).TotalDays + 1
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering historical re-aggregation");
            return StatusCode(500, "An error occurred while scheduling the re-aggregation job");
        }
    }

    /// <summary>
    /// Get job status for a Hangfire job
    /// </summary>
    /// <param name="jobId">Hangfire job ID</param>
    /// <returns>Job status information</returns>
    [HttpGet("jobs/{jobId}/status")]
    public ActionResult GetJobStatus(string jobId)
    {
        try
        {
            var jobStatus = JobStorage.Current.GetConnection().GetStateData(jobId);
            
            if (jobStatus == null)
            {
                return NotFound(new { Success = false, Message = "Job not found" });
            }

            return Ok(new
            {
                Success = true,
                JobId = jobId,
                State = jobStatus.Name,
                Data = jobStatus.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job status for job: {JobId}", jobId);
            return StatusCode(500, "An error occurred while retrieving job status");
        }
    }

    /// <summary>
    /// Schedule daily aggregation to run immediately for testing
    /// </summary>
    /// <returns>Job ID</returns>
    [HttpPost("aggregate/test")]
    public ActionResult TestDailyAggregation()
    {
        try
        {
            var jobId = BackgroundJob.Enqueue<SalesAggregationJobs>(
                x => x.RunDailySalesAggregation());

            return Ok(new
            {
                Success = true,
                Message = "Test daily aggregation job has been queued",
                JobId = jobId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scheduling test daily aggregation");
            return StatusCode(500, "An error occurred while scheduling the test job");
        }
    }
}
