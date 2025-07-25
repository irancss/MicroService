﻿using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;
using System.Linq;

namespace ProductService.Infrastructure.Services
{
    
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int? skip = null, int? take = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Products.AsNoTracking();

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task AddProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddProductsAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddRangeAsync(products, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteProductsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var productsToDelete = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToListAsync(cancellationToken);

            if (productsToDelete.Any())
            {
                _context.Products.RemoveRange(productsToDelete);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ProductExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> ProductExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Products.AnyAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int? skip = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking();
                // .Where(p => p.CategoryId == categoryId); // FIXME: Product class does not have CategoryId. Category filtering is disabled. The 'categoryId' parameter is currently unused.

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string name, int? skip = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.Name.Value.Contains(name)); // Case-sensitive search. Use p.Name.Value.ToLower().Contains(name.ToLower()) for case-insensitive.

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> GetProductCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products.CountAsync(cancellationToken);
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            // FIXME: Product class does not have CategoryId. Category filtering is disabled. The 'categoryId' parameter is currently unused.
            return await _context.Products.CountAsync(cancellationToken); // Original: .CountAsync(p => p.CategoryId == categoryId, cancellationToken);
        }
    }
}
