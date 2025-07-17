using ReportingService.Application.Queries.GetTopSellingProducts;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Interfaces;

public interface IReportingRepository
{
    // Order Facts
    Task<OrderFact?> GetOrderFactByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task AddOrderFactAsync(OrderFact orderFact, CancellationToken cancellationToken);

    // Daily Sales Aggregates
    Task<DailySalesAggregate?> GetDailySalesAggregateAsync(DateTime date, string currency, CancellationToken cancellationToken);
    Task<List<DailySalesAggregate>> GetDailySalesAggregatesRangeAsync(DateTime fromDate, DateTime toDate, string currency, CancellationToken cancellationToken);
    Task AddDailySalesAggregateAsync(DailySalesAggregate aggregate, CancellationToken cancellationToken);

    // Dimensions
    Task<DateDimension?> GetDateDimensionByDateAsync(DateTime date, CancellationToken cancellationToken);
    Task AddDateDimensionAsync(DateDimension dateDimension, CancellationToken cancellationToken);

    Task<CustomerDimension?> GetCustomerDimensionByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task AddCustomerDimensionAsync(CustomerDimension customerDimension, CancellationToken cancellationToken);

    Task<ProductDimension?> GetProductDimensionByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task AddProductDimensionAsync(ProductDimension productDimension, CancellationToken cancellationToken);

    // Analytical Queries
    Task<Domain.DTOs.DailySalesAggregation?> GetDailySalesAggregationAsync(DateTime date, string currency, CancellationToken cancellationToken);
    Task<List<Domain.DTOs.ProductAnalytics>> GetProductAnalyticsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        int topCount, 
        string? category, 
        string? brand, 
        string currency, 
        ProductRankingBy rankBy, 
        CancellationToken cancellationToken);

    // Unit of Work
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
