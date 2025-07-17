using Microsoft.EntityFrameworkCore;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;
using MarketingService.Infrastructure.Data;

namespace MarketingService.Infrastructure.Repositories;

public class UserSegmentRepository : IUserSegmentRepository
{
    private readonly MarketingDbContext _context;

    public UserSegmentRepository(MarketingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserSegment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegments
            .Include(s => s.Memberships)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserSegment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserSegments
            .Include(s => s.Memberships)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserSegment>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserSegments
            .Where(s => s.IsActive)
            .Include(s => s.Memberships)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserSegment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegments
            .Where(s => s.Memberships.Any(m => m.UserId == userId && m.IsCurrentlyActive))
            .Include(s => s.Memberships)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSegment> AddAsync(UserSegment segment, CancellationToken cancellationToken = default)
    {
        _context.UserSegments.Add(segment);
        await _context.SaveChangesAsync(cancellationToken);
        return segment;
    }

    public async Task UpdateAsync(UserSegment segment, CancellationToken cancellationToken = default)
    {
        _context.UserSegments.Update(segment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var segment = await GetByIdAsync(id, cancellationToken);
        if (segment != null)
        {
            _context.UserSegments.Remove(segment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
