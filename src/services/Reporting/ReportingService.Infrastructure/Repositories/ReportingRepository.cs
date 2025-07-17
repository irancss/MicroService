using Microsoft.EntityFrameworkCore;
using ReportingService.Application.Interfaces;
using ReportingService.Application.Queries.GetTopSellingProducts;
using ReportingService.Domain.DTOs;
using ReportingService.Domain.Entities;
using ReportingService.Infrastructure.Data;

namespace ReportingService.Infrastructure.Repositories;

public class ReportingRepository : IReportingRepository
{
    private readonly ReportingDbContext _context;

    public ReportingRepository(ReportingDbContext context)
    {
        _context = context;
    }

    // Order Facts
    public async Task<OrderFact?> GetOrderFactByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _context.OrderFacts
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
    }

    public async Task AddOrderFactAsync(OrderFact orderFact, CancellationToken cancellationToken)
    {
        await _context.OrderFacts.AddAsync(orderFact, cancellationToken);
    }

    // Daily Sales Aggregates
    public async Task<DailySalesAggregate?> GetDailySalesAggregateAsync(DateTime date, string currency, CancellationToken cancellationToken)
    {
        return await _context.DailySalesAggregates
            .FirstOrDefaultAsync(x => x.Date.Date == date.Date && x.Currency == currency, cancellationToken);
    }

    public async Task<List<DailySalesAggregate>> GetDailySalesAggregatesRangeAsync(DateTime fromDate, DateTime toDate, string currency, CancellationToken cancellationToken)
    {
        return await _context.DailySalesAggregates
            .Where(x => x.Date.Date >= fromDate.Date && 
                       x.Date.Date <= toDate.Date && 
                       x.Currency == currency)
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task AddDailySalesAggregateAsync(DailySalesAggregate aggregate, CancellationToken cancellationToken)
    {
        await _context.DailySalesAggregates.AddAsync(aggregate, cancellationToken);
    }

    // Dimensions
    public async Task<DateDimension?> GetDateDimensionByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        return await _context.DateDimensions
            .FirstOrDefaultAsync(x => x.Date.Date == date.Date, cancellationToken);
    }

    public async Task AddDateDimensionAsync(DateDimension dateDimension, CancellationToken cancellationToken)
    {
        await _context.DateDimensions.AddAsync(dateDimension, cancellationToken);
    }

    public async Task<CustomerDimension?> GetCustomerDimensionByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.CustomerDimensions
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
    }

    public async Task AddCustomerDimensionAsync(CustomerDimension customerDimension, CancellationToken cancellationToken)
    {
        await _context.CustomerDimensions.AddAsync(customerDimension, cancellationToken);
    }

    public async Task<ProductDimension?> GetProductDimensionByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.ProductDimensions
            .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
    }

    public async Task AddProductDimensionAsync(ProductDimension productDimension, CancellationToken cancellationToken)
    {
        await _context.ProductDimensions.AddAsync(productDimension, cancellationToken);
    }

    // Analytical Queries
    public async Task<DailySalesAggregation?> GetDailySalesAggregationAsync(DateTime date, string currency, CancellationToken cancellationToken)
    {
        var query = from of in _context.OrderFacts
                    join dd in _context.DateDimensions on of.DateDimensionId equals dd.Id
                    where dd.Date.Date == date.Date && of.Currency == currency
                    group of by new { dd.Date, of.Currency } into g
                    select new DailySalesAggregation
                    {
                        TotalRevenue = g.Sum(x => x.Revenue),
                        TotalTax = g.Sum(x => x.Tax),
                        TotalDiscount = g.Sum(x => x.Discount),
                        TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                        TotalItems = g.Sum(x => x.TotalItems),
                        AverageOrderValue = g.Sum(x => x.Revenue) / g.Select(x => x.OrderId).Distinct().Count()
                    };

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ProductAnalytics>> GetProductAnalyticsAsync(
        DateTime fromDate, 
        DateTime toDate, 
        int topCount, 
        string? category, 
        string? brand, 
        string currency, 
        ProductRankingBy rankBy, 
        CancellationToken cancellationToken)
    {
        // Complex analytical query joining fact and dimension tables
        var query = from of in _context.OrderFacts
                    join pd in _context.ProductDimensions on of.ProductDimensionId equals pd.Id
                    join dd in _context.DateDimensions on of.DateDimensionId equals dd.Id
                    where dd.Date >= fromDate && dd.Date <= toDate && of.Currency == currency
                    group new { of, pd } by new 
                    { 
                        pd.ProductId, 
                        pd.Name, 
                        pd.Category, 
                        pd.SubCategory, 
                        pd.Brand, 
                        pd.Price 
                    } into g
                    select new ProductAnalytics
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.Name,
                        Category = g.Key.Category,
                        SubCategory = g.Key.SubCategory,
                        Brand = g.Key.Brand,
                        ProductPrice = g.Key.Price,
                        TotalRevenue = g.Sum(x => x.of.Revenue),
                        TotalQuantitySold = g.Sum(x => x.of.TotalItems),
                        TotalOrders = g.Select(x => x.of.OrderId).Distinct().Count()
                    };

        // Apply filters
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(x => x.Category == category);
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(x => x.Brand == brand);
        }

        // Apply ordering based on ranking criteria
        query = rankBy switch
        {
            ProductRankingBy.Revenue => query.OrderByDescending(x => x.TotalRevenue),
            ProductRankingBy.Quantity => query.OrderByDescending(x => x.TotalQuantitySold),
            ProductRankingBy.OrderCount => query.OrderByDescending(x => x.TotalOrders),
            _ => query.OrderByDescending(x => x.TotalRevenue)
        };

        return await query.Take(topCount).ToListAsync(cancellationToken);
    }

    // Unit of Work
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
