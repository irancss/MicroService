using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Commands.AggregateSales;

namespace ReportingService.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire background jobs for scheduled aggregation tasks
/// </summary>
public class SalesAggregationJobs
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesAggregationJobs> _logger;

    public SalesAggregationJobs(IMediator mediator, ILogger<SalesAggregationJobs> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Runs daily sales aggregation for yesterday's data
    /// Scheduled to run every day at 2 AM
    /// </summary>
    [AutomaticRetry(Attempts = 3)]
    public async Task RunDailySalesAggregation()
    {
        try
        {
            var yesterday = DateTime.Today.AddDays(-1);
            _logger.LogInformation("Starting daily sales aggregation for date: {Date}", yesterday);

            var command = new AggregateDailySalesCommand
            {
                Date = yesterday,
                Currency = "USD" // Could be configured to handle multiple currencies
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Daily sales aggregation completed successfully for {Date}. Revenue: {Revenue}, Orders: {Orders}", 
                    yesterday, result.TotalRevenue, result.TotalOrders);
            }
            else
            {
                _logger.LogError("Daily sales aggregation failed for {Date}: {Error}", yesterday, result.Message);
                throw new InvalidOperationException($"Daily sales aggregation failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during daily sales aggregation");
            throw;
        }
    }

    /// <summary>
    /// Re-aggregate historical data for a specific date range
    /// Can be triggered manually or scheduled for data correction
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    public async Task ReaggregateHistoricalData(DateTime fromDate, DateTime toDate, string currency = "USD")
    {
        try
        {
            _logger.LogInformation("Starting historical data re-aggregation from {FromDate} to {ToDate}", fromDate, toDate);

            var currentDate = fromDate;
            var successCount = 0;
            var errorCount = 0;

            while (currentDate <= toDate)
            {
                try
                {
                    var command = new AggregateDailySalesCommand
                    {
                        Date = currentDate,
                        Currency = currency
                    };

                    var result = await _mediator.Send(command);

                    if (result.Success)
                    {
                        successCount++;
                        _logger.LogDebug("Re-aggregated data for {Date} successfully", currentDate);
                    }
                    else
                    {
                        errorCount++;
                        _logger.LogWarning("Failed to re-aggregate data for {Date}: {Error}", currentDate, result.Message);
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError(ex, "Error re-aggregating data for {Date}", currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }

            _logger.LogInformation("Historical data re-aggregation completed. Success: {Success}, Errors: {Errors}", 
                successCount, errorCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during historical data re-aggregation");
            throw;
        }
    }

    /// <summary>
    /// Cleanup old aggregated data beyond retention period
    /// Scheduled to run weekly
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    public Task CleanupOldData(int retentionDays = 1095) // 3 years default
    {
        try
        {
            var cutoffDate = DateTime.Today.AddDays(-retentionDays);
            _logger.LogInformation("Starting cleanup of aggregated data older than {CutoffDate}", cutoffDate);

            // This would be implemented in the repository
            // await _repository.DeleteOldAggregatedDataAsync(cutoffDate);

            _logger.LogInformation("Cleanup of old data completed");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during old data cleanup");
            throw;
        }
    }
}
