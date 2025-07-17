using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class ProductAttributeRepository : IProductAttribute // Renamed class for clarity
{
    private readonly ProductDbContext _context;

    public ProductAttributeRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductAttribute> GetByIdAsync(int id)
    {
        return await _context.ProductAttributes.FindAsync(id);
    }

    public async Task<IEnumerable<ProductAttribute>> GetAttributesByProductIdAsync(string productId)
    {
        return await _context.ProductAttributes
            .Where(pa => pa.ProductId == productId)
            .ToListAsync();
    }

    public async Task<ProductAttribute> AddAsync(ProductAttribute attribute)
    {
        await _context.ProductAttributes.AddAsync(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task UpdateAsync(ProductAttribute attribute)
    {
        _context.ProductAttributes.Update(attribute);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var attribute = await _context.ProductAttributes.FindAsync(id);
        if (attribute != null)
        {
            _context.ProductAttributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }
    }

    

    
}