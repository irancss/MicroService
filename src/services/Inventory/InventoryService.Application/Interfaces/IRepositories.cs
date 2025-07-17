using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces;

public interface IProductStockRepository
{
    Task<ProductStock?> GetByProductIdAsync(string productId);
    Task<List<ProductStock>> GetMultipleByProductIdsAsync(List<string> productIds);
    Task<ProductStock> CreateAsync(ProductStock productStock);
    Task<ProductStock> UpdateAsync(ProductStock productStock);
    Task DeleteAsync(string productId);
    Task<List<ProductStock>> GetProductsWithLowStockAsync();
    Task<List<ProductStock>> GetProductsWithExcessStockAsync();
    Task<bool> ExistsAsync(string productId);
}

public interface IStockTransactionRepository
{
    Task<StockTransaction> CreateAsync(StockTransaction transaction);
    Task<List<StockTransaction>> GetByProductIdAsync(string productId, int pageNumber = 1, int pageSize = 50);
    Task<List<StockTransaction>> GetRecentTransactionsAsync(int count = 100);
}
