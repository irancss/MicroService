using ShippingService.Domain.Entities;

namespace ShippingService.Domain.Repositories;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
    Task<IEnumerable<Shipment>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<Shipment>> GetByOrderIdAsync(string orderId);
    Task<IEnumerable<Shipment>> GetByDriverIdAsync(string driverId);
    Task<IEnumerable<Shipment>> GetActiveShipmentsAsync();
    Task AddAsync(Shipment shipment);
    Task UpdateAsync(Shipment shipment);
    Task DeleteAsync(Guid id);
}

public interface IShipmentReturnRepository
{
    Task<ShipmentReturn?> GetByIdAsync(Guid id);
    Task<ShipmentReturn?> GetByTrackingNumberAsync(string trackingNumber);
    Task<IEnumerable<ShipmentReturn>> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<ShipmentReturn>> GetByOriginalShipmentIdAsync(Guid originalShipmentId);
    Task<IEnumerable<ShipmentReturn>> GetPendingReturnsAsync();
    Task AddAsync(ShipmentReturn shipmentReturn);
    Task UpdateAsync(ShipmentReturn shipmentReturn);
    Task DeleteAsync(Guid id);
}
