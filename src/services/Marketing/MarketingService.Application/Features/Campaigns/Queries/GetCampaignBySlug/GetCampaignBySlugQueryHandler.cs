using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Application.Features.Campaigns.Queries.GetCampaignBySlug;

public class GetCampaignBySlugQueryHandler : IQueryHandler<GetCampaignBySlugQuery, CampaignDto?>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetCampaignBySlugQueryHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
    }

    public async Task<CampaignDto?> Handle(GetCampaignBySlugQuery request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetBySlugAsync(request.Slug, cancellationToken);
        
        if (campaign == null)
        {
            return null;
        }

        return new CampaignDto(
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
            campaign.CreatedBy);
    }
}
