using MediatR;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Commands.Shipments;

public record CreateShipmentCommand(
    string OrderId,
    string CustomerId,
    Guid ShippingMethodId,
    string OriginAddress,
    string DestinationAddress,
    string OriginCity,
    string DestinationCity,
    double OriginLatitude,
    double OriginLongitude,
    double DestinationLatitude,
    double DestinationLongitude,
    decimal Weight,
    decimal Width,
    decimal Height,
    decimal Length,
    decimal DeclaredValue,
    DateTime RequestedDeliveryDate
) : IRequest<CreateShipmentResult>;

public record CreateShipmentResult(
    Guid ShipmentId,
    string TrackingNumber,
    decimal TotalCost,
    DateTime EstimatedDeliveryDate,
    string OptimizedRoute,
    decimal EstimatedDistance,
    TimeSpan EstimatedDuration
);

public record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    ShipmentStatus Status,
    string? Notes = null,
    string? Location = null,
    string? UpdatedByUserId = null
) : IRequest;

public record AssignDriverCommand(
    Guid ShipmentId,
    string DriverId,
    string DriverName,
    string DriverPhone
) : IRequest;

public record OptimizeShipmentRouteCommand(
    Guid ShipmentId,
    bool AvoidTolls = false,
    bool AvoidHighways = false
) : IRequest<RouteOptimizationResult>;

public class RouteOptimizationResult
{
    public string OptimizedRoute { get; set; } = string.Empty;
    public decimal TotalDistance { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public decimal EstimatedFuelCost { get; set; }
    public DateTime EstimatedArrivalTime { get; set; }
}
