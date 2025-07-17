using MarketingService.Domain.Enums;
using MarketingService.Domain.ValueObjects;

namespace MarketingService.Domain.Entities;

public class Campaign
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Slug { get; private set; }
    public CampaignType Type { get; private set; }
    public CampaignStatus Status { get; private set; }
    public DateRange DateRange { get; private set; }
    public Money Budget { get; private set; }
    public CampaignMetrics Metrics { get; private set; }
    public List<Guid> TargetSegmentIds { get; private set; }
    public Guid? LandingPageId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }

    private Campaign() 
    {
        TargetSegmentIds = new List<Guid>();
        Name = string.Empty;
        Description = string.Empty;
        Slug = string.Empty;
        DateRange = new DateRange(DateTime.MinValue, DateTime.MinValue);
        Budget = Money.Zero();
        Metrics = new CampaignMetrics(0, 0, 0, Money.Zero(), Money.Zero());
        CreatedBy = string.Empty;
    }

    public Campaign(
        string name,
        string description,
        string slug,
        CampaignType type,
        DateRange dateRange,
        Money budget,
        string createdBy,
        List<Guid>? targetSegmentIds = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Type = type;
        Status = CampaignStatus.Draft;
        DateRange = dateRange ?? throw new ArgumentNullException(nameof(dateRange));
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        Metrics = new CampaignMetrics(0, 0, 0, Money.Zero(budget.Currency), Money.Zero(budget.Currency));
        TargetSegmentIds = targetSegmentIds ?? new List<Guid>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

        ValidateDateRange();
    }

    public void UpdateStatus(CampaignStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBudget(Money budget)
    {
        Budget = budget ?? throw new ArgumentNullException(nameof(budget));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignLandingPage(Guid landingPageId)
    {
        LandingPageId = landingPageId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTargetSegment(Guid segmentId)
    {
        if (!TargetSegmentIds.Contains(segmentId))
        {
            TargetSegmentIds.Add(segmentId);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTargetSegment(Guid segmentId)
    {
        TargetSegmentIds.Remove(segmentId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMetrics(CampaignMetrics metrics)
    {
        Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive => Status == CampaignStatus.Active && DateRange.IsActive;

    private void ValidateDateRange()
    {
        if (DateRange.StartDate >= DateRange.EndDate)
            throw new InvalidOperationException("Campaign end date must be after start date");
    }
}
