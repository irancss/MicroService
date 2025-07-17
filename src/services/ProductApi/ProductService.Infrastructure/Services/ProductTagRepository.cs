using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class ProductTagRepository : IProductTagRepository
{
    private readonly ProductDbContext _context;

    public ProductTagRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductTag> GetAsync(string productId, string tagId)
    {
        return await _context.ProductTags
            .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
    }

    public async Task<IEnumerable<ProductTag>> GetByProductIdAsync(string productId)
    {
        return await _context.ProductTags
            .Where(pt => pt.ProductId == productId)
            .Include(pt => pt.Tag) // Optionally include related Tag
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductTag>> GetByTagIdAsync(string tagId)
    {
        return await _context.ProductTags
            .Where(pt => pt.TagId == tagId)
            .Include(pt => pt.Product) // Optionally include related Product
            .ToListAsync();
    }

    public async Task AddAsync(ProductTag productTag)
    {
        await _context.ProductTags.AddAsync(productTag);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductTag productTag)
    {
        // For composite key entities, ensure it's tracked correctly or re-attach if necessary.
        // If ProductId and TagId are the composite key, EF might handle it with Update.
        // If you face issues, you might need to fetch then update properties.
        _context.ProductTags.Update(productTag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string productId, string tagId)
    {
        var productTag = await _context.ProductTags
            .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);
        if (productTag != null)
        {
            _context.ProductTags.Remove(productTag);
            await _context.SaveChangesAsync();
        }
    }
}