using Microsoft.EntityFrameworkCore;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly ShippingDbContext _context;

    public TimeSlotRepository(ShippingDbContext context)
    {
        _context = context;
    }

    // Template operations
    public async Task<TimeSlotTemplate?> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlotTemplates
            .Include(x => x.Bookings.Where(b => b.IsActive))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TimeSlotTemplate>> GetTemplatesByShippingMethodAsync(Guid shippingMethodId, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlotTemplates
            .Where(x => x.ShippingMethodId == shippingMethodId && x.IsActive)
            .Include(x => x.Bookings.Where(b => b.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeSlotTemplate> AddTemplateAsync(TimeSlotTemplate template, CancellationToken cancellationToken = default)
    {
        await _context.TimeSlotTemplates.AddAsync(template, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return template;
    }

    public async Task UpdateTemplateAsync(TimeSlotTemplate template, CancellationToken cancellationToken = default)
    {
        _context.TimeSlotTemplates.Update(template);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Booking operations
    public async Task<TimeSlotBooking?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlotBookings
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TimeSlotBooking>> GetBookingsByTemplateAndDateAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSlotBookings
            .Where(x => x.TimeSlotTemplateId == templateId && x.Date == date && x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeSlotBooking> AddBookingAsync(TimeSlotBooking booking, CancellationToken cancellationToken = default)
    {
        // Use transaction to ensure atomicity
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // Double-check availability before adding
            var template = await _context.TimeSlotTemplates
                .Include(x => x.Bookings.Where(b => b.Date == booking.Date && b.IsActive))
                .FirstOrDefaultAsync(x => x.Id == booking.TimeSlotTemplateId, cancellationToken);

            if (template == null)
                throw new InvalidOperationException("Time slot template not found");

            var currentBookings = template.Bookings.Count(b => b.Date == booking.Date && b.IsActive);
            if (currentBookings >= template.Capacity)
                throw new InvalidOperationException("Time slot is fully booked");

            await _context.TimeSlotBookings.AddAsync(booking, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            return booking;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateBookingAsync(TimeSlotBooking booking, CancellationToken cancellationToken = default)
    {
        _context.TimeSlotBookings.Update(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // Availability checks
    public async Task<bool> IsTimeSlotAvailableAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var template = await _context.TimeSlotTemplates
            .Include(x => x.Bookings.Where(b => b.Date == date && b.IsActive))
            .FirstOrDefaultAsync(x => x.Id == templateId && x.IsActive, cancellationToken);

        if (template == null)
            return false;

        return template.Bookings.Count < template.Capacity;
    }

    public async Task<int> GetAvailableCapacityAsync(Guid templateId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var template = await _context.TimeSlotTemplates
            .Include(x => x.Bookings.Where(b => b.Date == date && b.IsActive))
            .FirstOrDefaultAsync(x => x.Id == templateId && x.IsActive, cancellationToken);

        if (template == null)
            return 0;

        return Math.Max(0, template.Capacity - template.Bookings.Count);
    }
}
