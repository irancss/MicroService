using MarketingService.Domain.Entities;

namespace MarketingService.Domain.Interfaces;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Campaign?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Campaign>> GetBySegmentIdAsync(Guid segmentId, CancellationToken cancellationToken = default);
    Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ILandingPageRepository
{
    Task<LandingPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LandingPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IEnumerable<LandingPage>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<LandingPage>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<LandingPage> AddAsync(LandingPage landingPage, CancellationToken cancellationToken = default);
    Task UpdateAsync(LandingPage landingPage, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IUserSegmentRepository
{
    Task<UserSegment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSegment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSegment>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSegment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSegment> AddAsync(UserSegment segment, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSegment segment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IUserSegmentMembershipRepository
{
    Task<UserSegmentMembership?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSegmentMembership>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSegmentMembership>> GetBySegmentIdAsync(Guid segmentId, CancellationToken cancellationToken = default);
    Task<UserSegmentMembership?> GetByUserAndSegmentAsync(Guid userId, Guid segmentId, CancellationToken cancellationToken = default);
    Task<UserSegmentMembership> AddAsync(UserSegmentMembership membership, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSegmentMembership membership, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
