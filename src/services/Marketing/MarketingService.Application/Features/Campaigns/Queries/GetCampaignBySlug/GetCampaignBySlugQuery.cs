using MarketingService.Application.Common;
using MarketingService.Application.DTOs;

namespace MarketingService.Application.Features.Campaigns.Queries.GetCampaignBySlug;

public record GetCampaignBySlugQuery(string Slug) : IQuery<CampaignDto?>;
