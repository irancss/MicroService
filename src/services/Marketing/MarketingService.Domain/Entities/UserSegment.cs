using MarketingService.Domain.Enums;
using MarketingService.Domain.ValueObjects;

namespace MarketingService.Domain.Entities;

public class UserSegment
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public SegmentationType Type { get; private set; }
    public List<SegmentCriteria> Criteria { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }

    // Navigation properties
    public virtual ICollection<UserSegmentMembership> Memberships { get; private set; }

    private UserSegment()
    {
        Name = string.Empty;
        Description = string.Empty;
        Criteria = new List<SegmentCriteria>();
        CreatedBy = string.Empty;
        Memberships = new List<UserSegmentMembership>();
    }

    public UserSegment(
        string name,
        string description,
        SegmentationType type,
        List<SegmentCriteria> criteria,
        string createdBy)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        Memberships = new List<UserSegmentMembership>();
    }

    public void UpdateCriteria(List<SegmentCriteria> criteria)
    {
        Criteria = criteria ?? throw new ArgumentNullException(nameof(criteria));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        UpdatedAt = DateTime.UtcNow;
    }
}
