using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class ProductImageRepository : IProductImage // Renamed class for clarity, assuming IProductImage is for ProductImage entities
{
    private readonly ProductDbContext _context;

    public ProductImageRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImage> GetByIdAsync(int id)
    {
        return await _context.ProductImages.FindAsync(id);
    }

    public async Task<IEnumerable<ProductImage>> GetImagesByProductIdAsync(string productId)
    {
        return await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync();
    }

    public async Task<ProductImage> AddAsync(ProductImage productImage)
    {
        await _context.ProductImages.AddAsync(productImage);
        await _context.SaveChangesAsync();
        return productImage;
    }

    public async Task UpdateAsync(ProductImage productImage)
    {
        _context.ProductImages.Update(productImage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var productImage = await _context.ProductImages.FindAsync(id);
        if (productImage != null)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }
    }
}