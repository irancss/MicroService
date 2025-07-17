using MediatR;
using Microsoft.Extensions.Logging;
using ReportingService.Application.Interfaces;

namespace ReportingService.Application.Queries.GetTopSellingProducts;

public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, GetTopSellingProductsResponse>
{
    private readonly IReportingRepository _repository;
    private readonly ILogger<GetTopSellingProductsHandler> _logger;

    public GetTopSellingProductsHandler(
        IReportingRepository repository,
        ILogger<GetTopSellingProductsHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<GetTopSellingProductsResponse> Handle(
        GetTopSellingProductsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting top {Count} selling products from {FromDate} to {ToDate} ranked by {RankBy}", 
                request.TopCount, request.FromDate, request.ToDate, request.RankBy);

            // Get product analytics data using complex analytical queries
            var productAnalytics = await _repository.GetProductAnalyticsAsync(
                fromDate: request.FromDate,
                toDate: request.ToDate,
                topCount: request.TopCount,
                category: request.Category,
                brand: request.Brand,
                currency: request.Currency,
                rankBy: request.RankBy,
                cancellationToken: cancellationToken);

            // Calculate total revenue for percentage calculations
            var totalRevenue = productAnalytics.Sum(x => x.TotalRevenue);

            // Map to response objects with ranking and percentage calculations
            var topProducts = productAnalytics.Select((product, index) => new TopSellingProductItem
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Category = product.Category,
                SubCategory = product.SubCategory,
                Brand = product.Brand,
                ProductPrice = product.ProductPrice,
                TotalRevenue = product.TotalRevenue,
                TotalQuantitySold = product.TotalQuantitySold,
                TotalOrders = product.TotalOrders,
                AverageOrderQuantity = product.TotalOrders > 0 ? (decimal)product.TotalQuantitySold / product.TotalOrders : 0,
                RevenuePercentage = totalRevenue > 0 ? (product.TotalRevenue / totalRevenue) * 100 : 0,
                Rank = index + 1
            }).ToList();

            // Calculate summary
            var summary = CalculateSummary(productAnalytics, topProducts, totalRevenue);

            _logger.LogInformation("Retrieved {Count} top selling products", topProducts.Count);

            return new GetTopSellingProductsResponse
            {
                Products = topProducts,
                Summary = summary
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top selling products");
            throw;
        }
    }

    private static ProductAnalyticsSummary CalculateSummary(
        List<Domain.DTOs.ProductAnalytics> allProducts, 
        List<TopSellingProductItem> topProducts, 
        decimal totalRevenue)
    {
        var topProductsRevenue = topProducts.Sum(x => x.TotalRevenue);
        var topProductsRevenuePercentage = totalRevenue > 0 ? (topProductsRevenue / totalRevenue) * 100 : 0;

        return new ProductAnalyticsSummary
        {
            TotalRevenue = totalRevenue,
            TotalProductsSold = allProducts.Count,
            TotalQuantitySold = allProducts.Sum(x => x.TotalQuantitySold),
            TotalOrders = allProducts.Sum(x => x.TotalOrders),
            UniqueProducts = allProducts.Count,
            TopProductsRevenuePercentage = topProductsRevenuePercentage
        };
    }
}
