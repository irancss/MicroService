using DiscountService.Domain.Entities;

namespace DiscountService.Application.Interfaces;

/// <summary>
/// Repository interface for discount entity operations
/// </summary>
public interface IDiscountRepository
{
    /// <summary>
    /// Get discount by ID
    /// </summary>
    Task<Discount?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get discount by coupon code
    /// </summary>
    Task<Discount?> GetByCouponCodeAsync(string couponCode);
    
    /// <summary>
    /// Get all active automatic discounts
    /// </summary>
    Task<List<Discount>> GetActiveAutomaticDiscountsAsync();
    
    /// <summary>
    /// Get user-specific discounts
    /// </summary>
    Task<List<Discount>> GetUserSpecificDiscountsAsync(Guid userId);
    
    /// <summary>
    /// Get paginated list of discounts
    /// </summary>
    Task<(List<Discount> discounts, int totalCount)> GetPaginatedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    
    /// <summary>
    /// Add new discount
    /// </summary>
    Task<Discount> AddAsync(Discount discount);
    
    /// <summary>
    /// Update existing discount
    /// </summary>
    Task<Discount> UpdateAsync(Discount discount);
    
    /// <summary>
    /// Soft delete discount
    /// </summary>
    Task DeleteAsync(Guid id);
    
    /// <summary>
    /// Get user usage count for a specific discount
    /// </summary>
    Task<int> GetUserUsageCountAsync(Guid discountId, Guid userId);
    
    /// <summary>
    /// Update discount usage count
    /// </summary>
    Task UpdateUsageCountAsync(Guid discountId);
    
    /// <summary>
    /// Check if discount exists and is valid
    /// </summary>
    Task<bool> ExistsAndValidAsync(Guid id);
}

/// <summary>
/// Repository interface for discount usage history operations
/// </summary>
public interface IDiscountUsageHistoryRepository
{
    /// <summary>
    /// Add usage history record
    /// </summary>
    Task<DiscountUsageHistory> AddAsync(DiscountUsageHistory usageHistory);
    
    /// <summary>
    /// Get usage history for a discount
    /// </summary>
    Task<List<DiscountUsageHistory>> GetByDiscountIdAsync(Guid discountId, int pageNumber = 1, int pageSize = 10);
    
    /// <summary>
    /// Get usage history for a user
    /// </summary>
    Task<List<DiscountUsageHistory>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    
    /// <summary>
    /// Get usage count for user and discount
    /// </summary>
    Task<int> GetUsageCountAsync(Guid discountId, Guid userId);
}
