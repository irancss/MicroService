using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Application.Features.UserSegments.Queries.GetUserPersonalization;

public class GetUserPersonalizationQueryHandler : IQueryHandler<GetUserPersonalizationQuery, UserPersonalizationDto>
{
    private readonly IUserSegmentRepository _segmentRepository;
    private readonly ICampaignRepository _campaignRepository;

    public GetUserPersonalizationQueryHandler(
        IUserSegmentRepository segmentRepository,
        ICampaignRepository campaignRepository)
    {
        _segmentRepository = segmentRepository ?? throw new ArgumentNullException(nameof(segmentRepository));
        _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
    }

    public async Task<UserPersonalizationDto> Handle(GetUserPersonalizationQuery request, CancellationToken cancellationToken)
    {
        // Get user's segments
        var userSegments = await _segmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        var segmentDtos = userSegments.Select(s => new UserSegmentDto(
            s.Id,
            s.Name,
            s.Description,
            s.Type,
            s.IsActive,
            s.Criteria.Select(c => new SegmentCriteriaDto(c.Field, c.Operator, c.Value)).ToList(),
            s.CreatedAt)).ToList();

        // Get relevant campaigns for user's segments
        var relevantCampaigns = new List<CampaignDto>();
        
        foreach (var segment in userSegments)
        {
            var segmentCampaigns = await _campaignRepository.GetBySegmentIdAsync(segment.Id, cancellationToken);
            
            foreach (var campaign in segmentCampaigns.Where(c => c.IsActive))
            {
                // Avoid duplicates
                if (!relevantCampaigns.Any(rc => rc.Id == campaign.Id))
                {
                    relevantCampaigns.Add(new CampaignDto(
                        campaign.Id,
                        campaign.Name,
                        campaign.Description,
                        campaign.Slug,
                        campaign.Type,
                        campaign.Status,
                        campaign.DateRange.StartDate,
                        campaign.DateRange.EndDate,
                        campaign.Budget.Amount,
                        campaign.Budget.Currency,
                        campaign.Metrics.Impressions,
                        campaign.Metrics.Clicks,
                        campaign.Metrics.Conversions,
                        campaign.Metrics.Spent.Amount,
                        campaign.Metrics.Revenue.Amount,
                        campaign.TargetSegmentIds,
                        campaign.LandingPageId,
                        campaign.CreatedAt,
                        campaign.CreatedBy));
                }
            }
        }

        return new UserPersonalizationDto(
            request.UserId,
            segmentDtos,
            relevantCampaigns);
    }
}
