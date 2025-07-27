using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Domain.ValueObjects;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryAsync<Product>, IProductRepository
    {
        public ProductRepository(ProductDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Product?> GetByIdWithDetailsAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }

        public async Task<bool> IsSkuUniqueAsync(Sku sku, CancellationToken cancellationToken = default)
        {
            return !await _dbSet.AnyAsync(p => p.Sku == sku, cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid productId)
        {
            return await _dbSet.FindAsync(productId);
        }
    }
}
