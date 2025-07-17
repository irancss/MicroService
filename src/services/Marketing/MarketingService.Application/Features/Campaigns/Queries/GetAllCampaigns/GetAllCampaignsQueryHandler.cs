using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Interfaces;

namespace MarketingService.Application.Features.Campaigns.Queries.GetAllCampaigns;

public class GetAllCampaignsQueryHandler : IQueryHandler<GetAllCampaignsQuery, IEnumerable<CampaignDto>>
{
    private readonly ICampaignRepository _campaignRepository;

    public GetAllCampaignsQueryHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
    }

    public async Task<IEnumerable<CampaignDto>> Handle(GetAllCampaignsQuery request, CancellationToken cancellationToken)
    {
        var campaigns = await _campaignRepository.GetAllAsync(cancellationToken);

        return campaigns.Select(campaign => new CampaignDto(
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
