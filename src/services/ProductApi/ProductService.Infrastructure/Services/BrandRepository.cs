using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;
using Sieve.Models;

namespace ProductService.Infrastructure.Services
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ProductDbContext _context;

        public BrandRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Brand> GetByIdAsync(string id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> AddAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Brand>> GetPagedAndSortedAsync(SieveModel sieve)
        {
            // Pseudocode:
            // 1. Try to query the Brands DbSet.
            // 2. Apply ordering and paging if needed (currently just Take(10)).
            // 3. Catch and log exceptions.
            // 4. Return the result.

            try
            {
                return await _context.Brands
                    .Take(10)
                    .ToListAsync();
            }
            catch (FileNotFoundException ex) when (ex.Message.Contains("Microsoft.EntityFrameworkCore.Relational"))
            {
                Console.WriteLine("EF Core Relational assembly is missing. Please ensure the NuGet package 'Microsoft.EntityFrameworkCore.Relational' is installed.");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
