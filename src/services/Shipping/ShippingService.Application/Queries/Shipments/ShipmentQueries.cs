using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Queries.Shipments;

public record GetShipmentByIdQuery(Guid ShipmentId) : IRequest<ShipmentDto?>;

public record GetShipmentByTrackingNumberQuery(string TrackingNumber) : IRequest<ShipmentDto?>;

public record GetShipmentsByCustomerIdQuery(string CustomerId) : IRequest<IEnumerable<ShipmentDto>>;

public record GetShipmentsByDriverIdQuery(string DriverId) : IRequest<IEnumerable<ShipmentDto>>;

public record GetActiveShipmentsQuery() : IRequest<IEnumerable<ShipmentDto>>;

public record TrackShipmentQuery(string TrackingNumber) : IRequest<ShipmentTrackingDto?>;

public class ShipmentDto
{
    public Guid Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public ShipmentStatus Status { get; set; }
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public string? DeliveryDriverName { get; set; }
    public string? DeliveryDriverPhone { get; set; }
    public decimal EstimatedDistance { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShipmentTrackingDto
{
    public Guid ShipmentId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public ShipmentStatus CurrentStatus { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public List<TrackingEventDto> TrackingHistory { get; set; } = new();
}

public class TrackingEventDto
{
    public ShipmentStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}
