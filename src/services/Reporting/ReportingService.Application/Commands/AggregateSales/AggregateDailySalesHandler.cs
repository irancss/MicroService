using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Interfaces;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Commands.AggregateSales;

public class AggregateDailySalesHandler : IRequestHandler<AggregateDailySalesCommand, AggregateDailySalesResponse>
{
    private readonly IReportingRepository _repository;
    private readonly ILogger<AggregateDailySalesHandler> _logger;

    public AggregateDailySalesHandler(
        IReportingRepository repository,
        ILogger<AggregateDailySalesHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<AggregateDailySalesResponse> Handle(
        AggregateDailySalesCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Aggregating daily sales for date: {Date}, Currency: {Currency}", 
                request.Date, request.Currency);

            // Get aggregated data from order facts for the specific date
            var dailyAggregation = await _repository.GetDailySalesAggregationAsync(request.Date, request.Currency, cancellationToken);

            if (dailyAggregation == null)
            {
                _logger.LogInformation("No sales data found for date: {Date}", request.Date);
                return new AggregateDailySalesResponse
                {
                    Success = true,
                    Message = "No sales data found for the specified date"
                };
            }

            // Check if aggregate already exists
            var existingAggregate = await _repository.GetDailySalesAggregateAsync(request.Date, request.Currency, cancellationToken);

            DailySalesAggregate aggregate;
            if (existingAggregate != null)
            {
                // Update existing aggregate
                existingAggregate.UpdateAggregates(
                    totalRevenue: dailyAggregation.TotalRevenue,
                    totalTax: dailyAggregation.TotalTax,
                    totalDiscount: dailyAggregation.TotalDiscount,
                    totalOrders: dailyAggregation.TotalOrders,
                    totalItems: dailyAggregation.TotalItems,
                    averageOrderValue: dailyAggregation.AverageOrderValue
                );
                aggregate = existingAggregate;
                _logger.LogInformation("Updated existing daily sales aggregate for date: {Date}", request.Date);
            }
            else
            {
                // Create new aggregate
                aggregate = new DailySalesAggregate(
                    date: request.Date,
                    totalRevenue: dailyAggregation.TotalRevenue,
                    totalTax: dailyAggregation.TotalTax,
                    totalDiscount: dailyAggregation.TotalDiscount,
                    totalOrders: dailyAggregation.TotalOrders,
                    totalItems: dailyAggregation.TotalItems,
                    averageOrderValue: dailyAggregation.AverageOrderValue,
                    currency: request.Currency
                );

                await _repository.AddDailySalesAggregateAsync(aggregate, cancellationToken);
                _logger.LogInformation("Created new daily sales aggregate for date: {Date}", request.Date);
            }

            await _repository.SaveChangesAsync(cancellationToken);

            return new AggregateDailySalesResponse
            {
                Success = true,
                Message = "Daily sales aggregated successfully",
                AggregateId = aggregate.Id,
                TotalRevenue = aggregate.TotalRevenue,
                TotalOrders = aggregate.TotalOrders
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating daily sales for date: {Date}", request.Date);
            
            return new AggregateDailySalesResponse
            {
                Success = false,
                Message = $"Error aggregating daily sales: {ex.Message}"
            };
        }
    }
}
