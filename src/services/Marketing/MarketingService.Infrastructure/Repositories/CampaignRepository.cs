using Microsoft.EntityFrameworkCore;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;
using MarketingService.Infrastructure.Data;

namespace MarketingService.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly MarketingDbContext _context;

    public CampaignRepository(MarketingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Campaign?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns.FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<Campaign>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        return await _context.Campaigns
            .Where(c => c.Status == Domain.Enums.CampaignStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Campaign>> GetBySegmentIdAsync(Guid segmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns
            .Where(c => c.TargetSegmentIds.Contains(segmentId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync(cancellationToken);
        return campaign;
    }

    public async Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        _context.Campaigns.Update(campaign);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var campaign = await GetByIdAsync(id, cancellationToken);
        if (campaign != null)
        {
            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
