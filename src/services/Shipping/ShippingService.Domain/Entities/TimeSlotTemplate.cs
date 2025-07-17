using Shared.Kernel.Domain;
using DayOfWeekEnum = ShippingService.Domain.Enums.DayOfWeek;

namespace ShippingService.Domain.Entities;

public class TimeSlotTemplate : BaseEntity
{
    public Guid ShippingMethodId { get; private set; }
    public DayOfWeekEnum DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public ShippingMethod ShippingMethod { get; private set; } = null!;
    public ICollection<TimeSlotBooking> Bookings { get; private set; } = new List<TimeSlotBooking>();

    private TimeSlotTemplate() { } // For EF Core

    public TimeSlotTemplate(Guid shippingMethodId, DayOfWeekEnum dayOfWeek, TimeOnly startTime, TimeOnly endTime, int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(capacity));
        
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        ShippingMethodId = shippingMethodId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        Capacity = capacity;
        IsActive = true;
    }

    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(newCapacity));

        Capacity = newCapacity;
        SetUpdated();
    }

    public void UpdateTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        StartTime = startTime;
        EndTime = endTime;
        SetUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdated();
    }

    public int GetAvailableCapacity(DateOnly date)
    {
        var bookedCount = Bookings.Count(b => b.Date == date && b.IsActive);
        return Math.Max(0, Capacity - bookedCount);
    }
}
