using Microsoft.EntityFrameworkCore;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Repositories;

public class ShippingMethodRepository : IShippingMethodRepository
{
    private readonly ShippingDbContext _context;

    public ShippingMethodRepository(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<ShippingMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingMethods
            .Include(x => x.TimeSlotTemplates)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ShippingMethod>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ShippingMethods
            .Where(x => x.IsActive)
            .Include(x => x.TimeSlotTemplates.Where(t => t.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ShippingMethod> AddAsync(ShippingMethod shippingMethod, CancellationToken cancellationToken = default)
    {
        await _context.ShippingMethods.AddAsync(shippingMethod, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return shippingMethod;
    }

    public async Task UpdateAsync(ShippingMethod shippingMethod, CancellationToken cancellationToken = default)
    {
        _context.ShippingMethods.Update(shippingMethod);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shippingMethod = await _context.ShippingMethods.FindAsync(new object[] { id }, cancellationToken);
        if (shippingMethod != null)
        {
            shippingMethod.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShippingMethods.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
