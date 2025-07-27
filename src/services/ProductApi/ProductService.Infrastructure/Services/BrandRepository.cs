using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;
using Sieve.Models;

namespace ProductService.Infrastructure.Services
{
    public class BrandRepository : RepositoryAsync<Brand>, IBrandRepository
    {
        public BrandRepository(ProductDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Brand?> GetByIdAsync(Guid brandId)
        {
            return await _dbSet.FindAsync(brandId);
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(Brand brand)
        {
            await InsertAsync(brand);
        }

        public Task DeleteAsync(Brand brand)
        {
            DeleteAsync(brand);
            return Task.CompletedTask;
        }
    }
}
