using Cart.Application.Interfaces;
using Cart.Domain.Entities;
using Cart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure.Repositories
{
    public class NextPurchaseCartRepository : INextPurchaseCartRepository
    {
        private readonly CartDbContext _context;

        public NextPurchaseCartRepository(CartDbContext context)
        {
            _context = context;
        }

        public Task<NextPurchaseCart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            // Use Id since it's the same as UserId
            return _context.NextPurchaseCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == userId, cancellationToken);
        }

        public async Task<NextPurchaseCart> SaveAsync(NextPurchaseCart cart, CancellationToken cancellationToken = default)
        {
            var existingCart = await _context.NextPurchaseCarts
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == cart.Id, cancellationToken);

            if (existingCart == null)
            {
                _context.NextPurchaseCarts.Add(cart);
            }
            else
            {
                // EF Core tracking will handle updates
                _context.Entry(existingCart).CurrentValues.SetValues(cart);
                _context.Entry(existingCart).Collection(c => c.Items).CurrentValue = cart.Items;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return cart;
        }

        public async Task DeleteAsync(string userId, CancellationToken cancellationToken = default)
        {
            var cart = await GetByUserIdAsync(userId, cancellationToken);
            if (cart != null)
            {
                _context.NextPurchaseCarts.Remove(cart);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
