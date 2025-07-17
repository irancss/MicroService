using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

public class TimeSlotBooking : BaseEntity
{
    public Guid TimeSlotTemplateId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public string? OrderId { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public TimeSlotTemplate TimeSlotTemplate { get; private set; } = null!;

    private TimeSlotBooking() { } // For EF Core

    public TimeSlotBooking(Guid timeSlotTemplateId, DateOnly date, TimeOnly startTime, TimeOnly endTime, string customerId, string? orderId = null)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be null or empty", nameof(customerId));
        
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        TimeSlotTemplateId = timeSlotTemplateId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        CustomerId = customerId;
        OrderId = orderId;
        IsActive = true;
    }

    public void Cancel()
    {
        IsActive = false;
        SetUpdated();
    }

    public void UpdateOrderId(string orderId)
    {
        OrderId = orderId;
        SetUpdated();
    }
}
