using ShippingService.Domain.Enums;
using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

public class ReturnTracking : BaseEntity
{
    public Guid ReturnId { get; private set; }
    public ReturnStatus Status { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Notes { get; private set; }
    public string? UpdatedByUserId { get; private set; }

    protected ReturnTracking() { }

    public ReturnTracking(Guid returnId, ReturnStatus status, string? notes = null, string? updatedByUserId = null)
    {
        Id = Guid.NewGuid();
        ReturnId = returnId;
        Status = status;
        Timestamp = DateTime.UtcNow;
        Notes = notes;
        UpdatedByUserId = updatedByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
