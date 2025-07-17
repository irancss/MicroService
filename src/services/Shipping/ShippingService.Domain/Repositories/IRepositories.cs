using ShippingService.Domain.Entities;

namespace ShippingService.Domain.Repositories;

public interface IShippingMethodRepository
{
    Task<ShippingMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShippingMethod>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ShippingMethod> AddAsync(ShippingMethod shippingMethod, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShippingMethod shippingMethod, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ITimeSlotRepository
{
    Task<TimeSlotTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlotTemplate>> GetTemplatesByShippingMethodAsync(Guid shippingMethodId, CancellationToken cancellationToken = default);
    Task<TimeSlotTemplate> AddTemplateAsync(TimeSlotTemplate template, CancellationToken cancellationToken = default);
    Task UpdateTemplateAsync(TimeSlotTemplate template, CancellationToken cancellationToken = default);
    
    Task<TimeSlotBooking?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeSlotBooking>> GetBookingsByTemplateAndDateAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default);
    Task<TimeSlotBooking> AddBookingAsync(TimeSlotBooking booking, CancellationToken cancellationToken = default);
    Task UpdateBookingAsync(TimeSlotBooking booking, CancellationToken cancellationToken = default);
    
    Task<bool> IsTimeSlotAvailableAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default);
    Task<int> GetAvailableCapacityAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default);
}
