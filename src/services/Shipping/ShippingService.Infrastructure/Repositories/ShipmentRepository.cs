using Microsoft.EntityFrameworkCore;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ShippingDbContext _context;

    public ShipmentRepository(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id)
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
    }

    public async Task<IEnumerable<Shipment>> GetByCustomerIdAsync(string customerId)
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetByOrderIdAsync(string orderId)
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetByDriverIdAsync(string driverId)
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.DeliveryDriverId == driverId)
            .OrderBy(s => s.EstimatedDeliveryDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetActiveShipmentsAsync()
    {
        return await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.Status != Domain.Enums.ShipmentStatus.Delivered && 
                       s.Status != Domain.Enums.ShipmentStatus.Cancelled)
            .OrderBy(s => s.EstimatedDeliveryDate)
            .ToListAsync();
    }

    public async Task AddAsync(Shipment shipment)
    {
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Shipment shipment)
    {
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment != null)
        {
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
        }
    }
}

public class ShipmentReturnRepository : IShipmentReturnRepository
{
    private readonly ShippingDbContext _context;

    public ShipmentReturnRepository(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentReturn?> GetByIdAsync(Guid id)
    {
        return await _context.ShipmentReturns
            .Include(r => r.OriginalShipment)
            .Include(r => r.TrackingHistory)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<ShipmentReturn?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.ShipmentReturns
            .Include(r => r.OriginalShipment)
            .Include(r => r.TrackingHistory)
            .FirstOrDefaultAsync(r => r.ReturnTrackingNumber == trackingNumber);
    }

    public async Task<IEnumerable<ShipmentReturn>> GetByCustomerIdAsync(string customerId)
    {
        return await _context.ShipmentReturns
            .Include(r => r.OriginalShipment)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ShipmentReturn>> GetByOriginalShipmentIdAsync(Guid originalShipmentId)
    {
        return await _context.ShipmentReturns
            .Include(r => r.OriginalShipment)
            .Where(r => r.OriginalShipmentId == originalShipmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ShipmentReturn>> GetPendingReturnsAsync()
    {
        return await _context.ShipmentReturns
            .Include(r => r.OriginalShipment)
            .Where(r => r.Status == Domain.Enums.ReturnStatus.Requested)
            .OrderBy(r => r.RequestedDate)
            .ToListAsync();
    }

    public async Task AddAsync(ShipmentReturn shipmentReturn)
    {
        await _context.ShipmentReturns.AddAsync(shipmentReturn);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShipmentReturn shipmentReturn)
    {
        _context.ShipmentReturns.Update(shipmentReturn);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var shipmentReturn = await _context.ShipmentReturns.FindAsync(id);
        if (shipmentReturn != null)
        {
            _context.ShipmentReturns.Remove(shipmentReturn);
            await _context.SaveChangesAsync();
        }
    }
}
