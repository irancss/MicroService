using Microsoft.EntityFrameworkCore;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;
using MarketingService.Infrastructure.Data;

namespace MarketingService.Infrastructure.Repositories;

public class UserSegmentMembershipRepository : IUserSegmentMembershipRepository
{
    private readonly MarketingDbContext _context;

    public UserSegmentMembershipRepository(MarketingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserSegmentMembership?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegmentMemberships
            .Include(m => m.Segment)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserSegmentMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegmentMemberships
            .Include(m => m.Segment)
            .Where(m => m.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserSegmentMembership>> GetBySegmentIdAsync(Guid segmentId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegmentMemberships
            .Include(m => m.Segment)
            .Where(m => m.SegmentId == segmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSegmentMembership?> GetByUserAndSegmentAsync(Guid userId, Guid segmentId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSegmentMemberships
            .Include(m => m.Segment)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.SegmentId == segmentId, cancellationToken);
    }

    public async Task<UserSegmentMembership> AddAsync(UserSegmentMembership membership, CancellationToken cancellationToken = default)
    {
        _context.UserSegmentMemberships.Add(membership);
        await _context.SaveChangesAsync(cancellationToken);
        return membership;
    }

    public async Task UpdateAsync(UserSegmentMembership membership, CancellationToken cancellationToken = default)
    {
        _context.UserSegmentMemberships.Update(membership);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var membership = await GetByIdAsync(id, cancellationToken);
        if (membership != null)
        {
            _context.UserSegmentMemberships.Remove(membership);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
