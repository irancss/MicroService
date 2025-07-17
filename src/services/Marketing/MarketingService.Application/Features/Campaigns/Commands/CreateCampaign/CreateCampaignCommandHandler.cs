using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Entities;
using MarketingService.Domain.Interfaces;
using MarketingService.Domain.ValueObjects;

namespace MarketingService.Application.Features.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandHandler : ICommandHandler<CreateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;

    public CreateCampaignCommandHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
    }

    public async Task<CampaignDto> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        // Check if slug already exists
        var existingCampaign = await _campaignRepository.GetBySlugAsync(request.Slug, cancellationToken);
        if (existingCampaign != null)
        {
            throw new InvalidOperationException($"Campaign with slug '{request.Slug}' already exists");
        }

        // Create value objects
        var dateRange = new DateRange(request.StartDate, request.EndDate);
        var budget = new Money(request.BudgetAmount, request.BudgetCurrency);

        // Create campaign entity
        var campaign = new Campaign(
            request.Name,
            request.Description,
            request.Slug,
            request.Type,
            dateRange,
            budget,
            request.CreatedBy,
            request.TargetSegmentIds);

        // Save campaign
        var savedCampaign = await _campaignRepository.AddAsync(campaign, cancellationToken);

        // Return DTO
        return new CampaignDto(
            savedCampaign.Id,
            savedCampaign.Name,
            savedCampaign.Description,
            savedCampaign.Slug,
            savedCampaign.Type,
            savedCampaign.Status,
            savedCampaign.DateRange.StartDate,
            savedCampaign.DateRange.EndDate,
            savedCampaign.Budget.Amount,
            savedCampaign.Budget.Currency,
            savedCampaign.Metrics.Impressions,
            savedCampaign.Metrics.Clicks,
            savedCampaign.Metrics.Conversions,
            savedCampaign.Metrics.Spent.Amount,
            savedCampaign.Metrics.Revenue.Amount,
            savedCampaign.TargetSegmentIds,
            savedCampaign.LandingPageId,
            savedCampaign.CreatedAt,
            savedCampaign.CreatedBy);
    }
}
