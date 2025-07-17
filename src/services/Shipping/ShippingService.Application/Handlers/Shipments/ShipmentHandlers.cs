using MediatR;
using ShippingService.Application.Commands.Shipments;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Repositories;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Handlers.Shipments;

public class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, CreateShipmentResult>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShippingMethodRepository _shippingMethodRepository;
    private readonly IRouteOptimizationService _routeOptimizationService;
    private readonly INotificationService _notificationService;

    public CreateShipmentHandler(
        IShipmentRepository shipmentRepository,
        IShippingMethodRepository shippingMethodRepository,
        IRouteOptimizationService routeOptimizationService,
        INotificationService notificationService)
    {
        _shipmentRepository = shipmentRepository;
        _shippingMethodRepository = shippingMethodRepository;
        _routeOptimizationService = routeOptimizationService;
        _notificationService = notificationService;
    }

    public async Task<CreateShipmentResult> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        // Get shipping method and calculate cost
        var shippingMethod = await _shippingMethodRepository.GetByIdAsync(request.ShippingMethodId);
        if (shippingMethod == null)
            throw new ArgumentException("Shipping method not found");

        // Calculate total cost (simplified - should use cost calculation service)
        var totalCost = shippingMethod.BaseCost; // Add cost rules calculation here

        // Create shipment
        var shipment = new Shipment(
            request.OrderId,
            request.CustomerId,
            request.ShippingMethodId,
            request.OriginAddress,
            request.DestinationAddress,
            request.OriginCity,
            request.DestinationCity,
            request.OriginLatitude,
            request.OriginLongitude,
            request.DestinationLatitude,
            request.DestinationLongitude,
            request.Weight,
            request.Width,
            request.Height,
            request.Length,
            request.DeclaredValue,
            totalCost,
            request.RequestedDeliveryDate);

        // Optimize route
        var routeRequest = new RouteOptimizationRequest
        {
            OriginAddress = request.OriginAddress,
            DestinationAddress = request.DestinationAddress,
            OriginLatitude = request.OriginLatitude,
            OriginLongitude = request.OriginLongitude,
            DestinationLatitude = request.DestinationLatitude,
            DestinationLongitude = request.DestinationLongitude,
            PreferredDeliveryTime = request.RequestedDeliveryDate
        };

        var routeResult = await _routeOptimizationService.OptimizeRouteAsync(routeRequest);
        shipment.SetOptimizedRoute(routeResult.OptimizedRoute, routeResult.TotalDistance, routeResult.EstimatedDuration);

        await _shipmentRepository.AddAsync(shipment);

        // Send notification
        await _notificationService.SendShipmentStatusUpdateAsync(
            request.CustomerId,
            shipment.TrackingNumber!,
            shipment.Status,
            "Your shipment has been created and is being processed.");

        return new CreateShipmentResult(
            shipment.Id,
            shipment.TrackingNumber!,
            totalCost,
            shipment.EstimatedDeliveryDate,
            routeResult.OptimizedRoute,
            routeResult.TotalDistance,
            routeResult.EstimatedDuration);
    }
}

public class UpdateShipmentStatusHandler : IRequestHandler<UpdateShipmentStatusCommand>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly INotificationService _notificationService;

    public UpdateShipmentStatusHandler(
        IShipmentRepository shipmentRepository,
        INotificationService notificationService)
    {
        _shipmentRepository = shipmentRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId);
        if (shipment == null)
            throw new ArgumentException("Shipment not found");

        shipment.UpdateStatus(request.Status, request.Notes);
        await _shipmentRepository.UpdateAsync(shipment);

        // Send notification
        await _notificationService.SendShipmentStatusUpdateAsync(
            shipment.CustomerId,
            shipment.TrackingNumber!,
            request.Status,
            request.Notes);
    }
}

public class AssignDriverHandler : IRequestHandler<AssignDriverCommand>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly INotificationService _notificationService;

    public AssignDriverHandler(
        IShipmentRepository shipmentRepository,
        INotificationService notificationService)
    {
        _shipmentRepository = shipmentRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId);
        if (shipment == null)
            throw new ArgumentException("Shipment not found");

        shipment.AssignDriver(request.DriverId, request.DriverName, request.DriverPhone);
        await _shipmentRepository.UpdateAsync(shipment);

        // Send notification to customer
        await _notificationService.SendShipmentStatusUpdateAsync(
            shipment.CustomerId,
            shipment.TrackingNumber!,
            shipment.Status,
            $"Driver {request.DriverName} has been assigned to your shipment. Contact: {request.DriverPhone}");
    }
}
