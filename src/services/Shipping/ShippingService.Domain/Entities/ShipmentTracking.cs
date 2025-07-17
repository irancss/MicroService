using ShippingService.Domain.Enums;
using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

public class ShipmentTracking : BaseEntity
{
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? Location { get; private set; }
    public string? Notes { get; private set; }
    public string? UpdatedByUserId { get; private set; }

    protected ShipmentTracking() { }

    public ShipmentTracking(Guid shipmentId, ShipmentStatus status, string? notes = null, string? location = null, string? updatedByUserId = null)
    {
        Id = Guid.NewGuid();
        ShipmentId = shipmentId;
        Status = status;
        Timestamp = DateTime.UtcNow;
        Notes = notes;
        Location = location;
        UpdatedByUserId = updatedByUserId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(string location)
    {
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
