namespace MarketingService.Domain.Entities;

public class UserSegmentMembership
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid SegmentId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public string AssignedBy { get; private set; }

    // Navigation properties
    public virtual UserSegment? Segment { get; private set; }

    private UserSegmentMembership()
    {
        AssignedBy = string.Empty;
    }

    public UserSegmentMembership(
        Guid userId,
        Guid segmentId,
        string assignedBy,
        DateTime? expiresAt = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        SegmentId = segmentId;
        AssignedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IsActive = true;
        AssignedBy = assignedBy ?? throw new ArgumentNullException(nameof(assignedBy));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool IsCurrentlyActive => IsActive && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    public void ExtendExpiry(DateTime newExpiryDate)
    {
        if (newExpiryDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Expiry date must be in the future");
        
        ExpiresAt = newExpiryDate;
    }
}
