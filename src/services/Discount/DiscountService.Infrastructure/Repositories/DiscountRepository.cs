using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DiscountService.Application.Interfaces;
using DiscountService.Domain.Entities;
using DiscountService.Infrastructure.Data;

namespace DiscountService.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Discount entity
/// </summary>
public class DiscountRepository : IDiscountRepository
{
    private readonly DiscountDbContext _context;
    private readonly ILogger<DiscountRepository> _logger;

    public DiscountRepository(DiscountDbContext context, ILogger<DiscountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Discount?> GetByIdAsync(Guid id)
    {
        return await _context.Discounts
            .Include(d => d.UsageHistory)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Discount?> GetByCouponCodeAsync(string couponCode)
    {
        return await _context.Discounts
            .FirstOrDefaultAsync(d => d.CouponCode == couponCode && d.IsActive);
    }

    public async Task<List<Discount>> GetActiveAutomaticDiscountsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Where(d => d.IsActive && 
                       d.IsAutomatic && 
                       d.StartDate <= now && 
                       d.EndDate >= now &&
                       (!d.MaxTotalUsage.HasValue || d.CurrentTotalUsage < d.MaxTotalUsage.Value))
            .ToListAsync();
    }

    public async Task<List<Discount>> GetUserSpecificDiscountsAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .Where(d => d.IsActive && 
                       d.UserId == userId && 
                       d.StartDate <= now && 
                       d.EndDate >= now &&
                       (!d.MaxTotalUsage.HasValue || d.CurrentTotalUsage < d.MaxTotalUsage.Value))
            .ToListAsync();
    }

    public async Task<(List<Discount> discounts, int totalCount)> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null)
    {
        var query = _context.Discounts.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(d => d.Name.Contains(searchTerm) || 
                                   d.Description.Contains(searchTerm) ||
                                   (d.CouponCode != null && d.CouponCode.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();
        
        var discounts = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (discounts, totalCount);
    }

    public async Task<Discount> AddAsync(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task<Discount> UpdateAsync(Discount discount)
    {
        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
        return discount;
    }

    public async Task DeleteAsync(Guid id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount != null)
        {
            discount.IsActive = false;
            discount.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetUserUsageCountAsync(Guid discountId, Guid userId)
    {
        return await _context.DiscountUsageHistories
            .CountAsync(h => h.DiscountId == discountId && h.UserId == userId);
    }

    public async Task UpdateUsageCountAsync(Guid discountId)
    {
        var discount = await _context.Discounts.FindAsync(discountId);
        if (discount != null)
        {
            discount.CurrentTotalUsage++;
            discount.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAndValidAsync(Guid id)
    {
        var now = DateTime.UtcNow;
        return await _context.Discounts
            .AnyAsync(d => d.Id == id && 
                          d.IsActive && 
                          d.StartDate <= now && 
                          d.EndDate >= now);
    }
}

/// <summary>
/// Repository implementation for DiscountUsageHistory entity
/// </summary>
public class DiscountUsageHistoryRepository : IDiscountUsageHistoryRepository
{
    private readonly DiscountDbContext _context;
    private readonly ILogger<DiscountUsageHistoryRepository> _logger;

    public DiscountUsageHistoryRepository(DiscountDbContext context, ILogger<DiscountUsageHistoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DiscountUsageHistory> AddAsync(DiscountUsageHistory usageHistory)
    {
        _context.DiscountUsageHistories.Add(usageHistory);
        await _context.SaveChangesAsync();
        return usageHistory;
    }

    public async Task<List<DiscountUsageHistory>> GetByDiscountIdAsync(Guid discountId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.DiscountUsageHistories
            .Include(h => h.Discount)
            .Where(h => h.DiscountId == discountId)
            .OrderByDescending(h => h.UsedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<DiscountUsageHistory>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        return await _context.DiscountUsageHistories
            .Include(h => h.Discount)
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.UsedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUsageCountAsync(Guid discountId, Guid userId)
    {
        return await _context.DiscountUsageHistories
            .CountAsync(h => h.DiscountId == discountId && h.UserId == userId);
    }
}
