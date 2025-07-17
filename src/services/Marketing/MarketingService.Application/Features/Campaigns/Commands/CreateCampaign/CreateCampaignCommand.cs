using MarketingService.Application.Common;
using MarketingService.Application.DTOs;
using MarketingService.Domain.Enums;

namespace MarketingService.Application.Features.Campaigns.Commands.CreateCampaign;

public record CreateCampaignCommand(
    string Name,
    string Description,
    string Slug,
    CampaignType Type,
    DateTime StartDate,
    DateTime EndDate,
    decimal BudgetAmount,
    string BudgetCurrency,
    List<Guid>? TargetSegmentIds,
    string CreatedBy) : ICommand<CampaignDto>;
