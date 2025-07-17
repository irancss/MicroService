using MarketingService.Application.Common;
using MarketingService.Application.DTOs;

namespace MarketingService.Application.Features.Campaigns.Queries.GetAllCampaigns;

public record GetAllCampaignsQuery() : IQuery<IEnumerable<CampaignDto>>;
