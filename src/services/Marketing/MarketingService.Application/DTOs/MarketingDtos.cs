using MarketingService.Domain.Enums;

namespace MarketingService.Application.DTOs;

public record CampaignDto(
    Guid Id,
    string Name,
    string Description,
    string Slug,
    CampaignType Type,
    CampaignStatus Status,
    DateTime StartDate,
    DateTime EndDate,
    decimal BudgetAmount,
    string BudgetCurrency,
    int Impressions,
    int Clicks,
    int Conversions,
    decimal Spent,
    decimal Revenue,
    List<Guid> TargetSegmentIds,
    Guid? LandingPageId,
    DateTime CreatedAt,
    string CreatedBy);

public record LandingPageDto(
    Guid Id,
    string Title,
    string Slug,
    string Content,
    string MetaDescription,
    string MetaKeywords,
    LandingPageStatus Status,
    string? CustomCss,
    string? CustomJs,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record UserSegmentDto(
    Guid Id,
    string Name,
    string Description,
    SegmentationType Type,
    bool IsActive,
    List<SegmentCriteriaDto> Criteria,
    DateTime CreatedAt);

public record SegmentCriteriaDto(
    string Field,
    string Operator,
    string Value);

public record UserPersonalizationDto(
    Guid UserId,
    List<UserSegmentDto> Segments,
    List<CampaignDto> RelevantCampaigns);
