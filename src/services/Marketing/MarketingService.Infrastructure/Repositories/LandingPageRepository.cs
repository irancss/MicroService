using Microsoft.EntityFrameworkCore;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;
using MarketingService.Infrastructure.Data;

namespace MarketingService.Infrastructure.Repositories;

public class LandingPageRepository : ILandingPageRepository
{
    private readonly MarketingDbContext _context;

    public LandingPageRepository(MarketingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LandingPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LandingPages.FirstOrDefaultAsync(lp => lp.Id == id, cancellationToken);
    }

    public async Task<LandingPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.LandingPages.FirstOrDefaultAsync(lp => lp.Slug == slug, cancellationToken);
    }

    public async Task<IEnumerable<LandingPage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LandingPages.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<LandingPage>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LandingPages
            .Where(lp => lp.Status == Domain.Enums.LandingPageStatus.Published)
            .ToListAsync(cancellationToken);
    }

    public async Task<LandingPage> AddAsync(LandingPage landingPage, CancellationToken cancellationToken = default)
    {
        _context.LandingPages.Add(landingPage);
        await _context.SaveChangesAsync(cancellationToken);
        return landingPage;
    }

    public async Task UpdateAsync(LandingPage landingPage, CancellationToken cancellationToken = default)
    {
        _context.LandingPages.Update(landingPage);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var landingPage = await GetByIdAsync(id, cancellationToken);
        if (landingPage != null)
        {
            _context.LandingPages.Remove(landingPage);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
