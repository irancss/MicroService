using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class ProductSpecificationRepository : IProductSpecification // Renamed class for clarity
{
    private readonly ProductDbContext _context;

    public ProductSpecificationRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductSpecification> GetByIdAsync(int id)
    {
        return await _context.ProductSpecifications.FindAsync(id);
    }

    public async Task<IEnumerable<ProductSpecification>> GetSpecificationsByProductIdAsync(string productId)
    {
        return await _context.ProductSpecifications
            .Where(ps => ps.ProductId == productId)
            .ToListAsync();
    }

    public async Task<ProductSpecification> AddAsync(ProductSpecification specification)
    {
        await _context.ProductSpecifications.AddAsync(specification);
        await _context.SaveChangesAsync();
        return specification;
    }

    public async Task UpdateAsync(ProductSpecification specification)
    {
        _context.ProductSpecifications.Update(specification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var specification = await _context.ProductSpecifications.FindAsync(id);
        if (specification != null)
        {
            _context.ProductSpecifications.Remove(specification);
            await _context.SaveChangesAsync();
        }
    }
}