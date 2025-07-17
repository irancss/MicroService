using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class ProductVariantStockRepository : IProductVariantStock // Renamed class for clarity
{
    private readonly ProductDbContext _context;

    public ProductVariantStockRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductVariantStock> GetByIdAsync(int id)
    {
        return await _context.ProductVariantStocks.FindAsync(id);
    }

    public async Task<ProductVariantStock> GetByProductVariantIdAsync(string productVariantId)
    {
        return await _context.ProductVariantStocks
            .FirstOrDefaultAsync(pvs => pvs.ProductVariantId == productVariantId);
    }

    public async Task<IEnumerable<ProductVariantStock>> GetStockByProductIdAsync(string productId)
    {
        // This assumes ProductVariantStock has a navigation property or direct/indirect link to ProductId.
        // If ProductVariantStock is linked via ProductVariant, which is linked to Product:
        return await _context.ProductVariantStocks
            .Include(pvs => pvs.ProductVariant) // Assuming ProductVariantStock has a ProductVariant navigation property
            .Where(pvs => pvs.ProductVariant.ProductId == productId)
            .ToListAsync();
        // If ProductVariantStock has a direct ProductId, then:
        // return await _context.ProductVariantStocks
        //                      .Where(pvs => pvs.ProductId == productId) // Assuming direct ProductId
        //                      .ToListAsync();
        // Adjust the query based on your actual entity relationships.
    }

    public async Task<ProductVariantStock> AddAsync(ProductVariantStock stock)
    {
        await _context.ProductVariantStocks.AddAsync(stock);
        await _context.SaveChangesAsync();
        return stock;
    }

    public async Task UpdateAsync(ProductVariantStock stock)
    {
        _context.ProductVariantStocks.Update(stock);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var stock = await _context.ProductVariantStocks.FindAsync(id);
        if (stock != null)
        {
            _context.ProductVariantStocks.Remove(stock);
            await _context.SaveChangesAsync();
        }
    }
}