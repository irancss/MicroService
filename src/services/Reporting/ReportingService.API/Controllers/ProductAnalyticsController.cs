using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Queries.GetTopSellingProducts;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductAnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductAnalyticsController> _logger;

    public ProductAnalyticsController(IMediator mediator, ILogger<ProductAnalyticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get top selling products for a specified date range
    /// </summary>
    /// <param name="fromDate">Start date (YYYY-MM-DD)</param>
    /// <param name="toDate">End date (YYYY-MM-DD)</param>
    /// <param name="topCount">Number of top products to return (default: 10, max: 100)</param>
    /// <param name="category">Filter by product category (optional)</param>
    /// <param name="brand">Filter by product brand (optional)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="rankBy">Ranking criteria: Revenue, Quantity, or OrderCount (default: Revenue)</param>
    /// <returns>Top selling products with analytics data</returns>
    [HttpGet("top-selling")]
    public async Task<ActionResult<GetTopSellingProductsResponse>> GetTopSellingProducts(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] int topCount = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] string currency = "USD",
        [FromQuery] ProductRankingBy rankBy = ProductRankingBy.Revenue)
    {
        try
        {
            var query = new GetTopSellingProductsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                TopCount = Math.Min(topCount, 100), // Limit to 100
                Category = category,
                Brand = brand,
                Currency = currency,
                RankBy = rankBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top selling products");
            return StatusCode(500, "An error occurred while retrieving the product analytics");
        }
    }

    /// <summary>
    /// Get top selling products for last 30 days
    /// </summary>
    /// <param name="topCount">Number of top products to return (default: 10)</param>
    /// <param name="category">Filter by product category (optional)</param>
    /// <param name="brand">Filter by product brand (optional)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="rankBy">Ranking criteria (default: Revenue)</param>
    /// <returns>Top selling products for last 30 days</returns>
    [HttpGet("top-selling/last30days")]
    public async Task<ActionResult<GetTopSellingProductsResponse>> GetTopSellingProductsLast30Days(
        [FromQuery] int topCount = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? brand = null,
        [FromQuery] string currency = "USD",
        [FromQuery] ProductRankingBy rankBy = ProductRankingBy.Revenue)
    {
        try
        {
            var toDate = DateTime.Today;
            var fromDate = toDate.AddDays(-30);

            var query = new GetTopSellingProductsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                TopCount = Math.Min(topCount, 100),
                Category = category,
                Brand = brand,
                Currency = currency,
                RankBy = rankBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top selling products for last 30 days");
            return StatusCode(500, "An error occurred while retrieving the product analytics");
        }
    }

    /// <summary>
    /// Get product analytics by category
    /// </summary>
    /// <param name="fromDate">Start date (YYYY-MM-DD)</param>
    /// <param name="toDate">End date (YYYY-MM-DD)</param>
    /// <param name="category">Product category</param>
    /// <param name="topCount">Number of top products to return (default: 20)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="rankBy">Ranking criteria (default: Revenue)</param>
    /// <returns>Top products in the specified category</returns>
    [HttpGet("by-category/{category}")]
    public async Task<ActionResult<GetTopSellingProductsResponse>> GetProductsByCategory(
        string category,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] int topCount = 20,
        [FromQuery] string currency = "USD",
        [FromQuery] ProductRankingBy rankBy = ProductRankingBy.Revenue)
    {
        try
        {
            var query = new GetTopSellingProductsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                TopCount = Math.Min(topCount, 100),
                Category = category,
                Currency = currency,
                RankBy = rankBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by category {Category}", category);
            return StatusCode(500, "An error occurred while retrieving the product analytics");
        }
    }

    /// <summary>
    /// Get product analytics by brand
    /// </summary>
    /// <param name="fromDate">Start date (YYYY-MM-DD)</param>
    /// <param name="toDate">End date (YYYY-MM-DD)</param>
    /// <param name="brand">Product brand</param>
    /// <param name="topCount">Number of top products to return (default: 20)</param>
    /// <param name="currency">Currency code (default: USD)</param>
    /// <param name="rankBy">Ranking criteria (default: Revenue)</param>
    /// <returns>Top products for the specified brand</returns>
    [HttpGet("by-brand/{brand}")]
    public async Task<ActionResult<GetTopSellingProductsResponse>> GetProductsByBrand(
        string brand,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] int topCount = 20,
        [FromQuery] string currency = "USD",
        [FromQuery] ProductRankingBy rankBy = ProductRankingBy.Revenue)
    {
        try
        {
            var query = new GetTopSellingProductsQuery
            {
                FromDate = fromDate,
                ToDate = toDate,
                TopCount = Math.Min(topCount, 100),
                Brand = brand,
                Currency = currency,
                RankBy = rankBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by brand {Brand}", brand);
            return StatusCode(500, "An error occurred while retrieving the product analytics");
        }
    }
}
