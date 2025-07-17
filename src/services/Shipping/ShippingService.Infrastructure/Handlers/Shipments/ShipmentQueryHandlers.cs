using MediatR;
using Microsoft.EntityFrameworkCore;
using ShippingService.Application.Queries.Shipments;
using ShippingService.Domain.Enums;
using ShippingService.Infrastructure.Data;

namespace ShippingService.Infrastructure.Handlers.Shipments;

public class GetActiveShipmentsQueryHandler : IRequestHandler<GetActiveShipmentsQuery, IEnumerable<ShipmentDto>>
{
    private readonly ShippingDbContext _context;

    public GetActiveShipmentsQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShipmentDto>> Handle(GetActiveShipmentsQuery request, CancellationToken cancellationToken)
    {
        var activeStatuses = new[]
        {
            ShipmentStatus.Created,
            ShipmentStatus.PickupScheduled,
            ShipmentStatus.PickedUp,
            ShipmentStatus.InTransit,
            ShipmentStatus.OutForDelivery
        };

        var activeShipments = await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => activeStatuses.Contains(s.Status))
            .Select(s => new ShipmentDto
            {
                Id = s.Id,
                OrderId = s.OrderId,
                CustomerId = s.CustomerId,
                TrackingNumber = s.TrackingNumber ?? string.Empty,
                Status = s.Status,
                OriginAddress = s.OriginAddress,
                DestinationAddress = s.DestinationAddress,
                Weight = s.Weight,
                TotalCost = s.TotalCost,
                EstimatedDeliveryDate = s.EstimatedDeliveryDate,
                ActualDeliveryDate = s.ActualDeliveryDate,
                DeliveryDriverName = s.DeliveryDriverName,
                DeliveryDriverPhone = s.DeliveryDriverPhone,
                EstimatedDistance = s.EstimatedDistance,
                EstimatedDuration = s.EstimatedDuration,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return activeShipments;
    }
}

public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, ShipmentDto?>
{
    private readonly ShippingDbContext _context;

    public GetShipmentByIdQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentDto?> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _context.Shipments
            .Include(s => s.ShippingMethod)
            .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

        if (shipment == null)
            return null;

        return new ShipmentDto
        {
            Id = shipment.Id,
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            TrackingNumber = shipment.TrackingNumber ?? string.Empty,
            Status = shipment.Status,
            OriginAddress = shipment.OriginAddress,
            DestinationAddress = shipment.DestinationAddress,
            Weight = shipment.Weight,
            TotalCost = shipment.TotalCost,
            EstimatedDeliveryDate = shipment.EstimatedDeliveryDate,
            ActualDeliveryDate = shipment.ActualDeliveryDate,
            DeliveryDriverName = shipment.DeliveryDriverName,
            DeliveryDriverPhone = shipment.DeliveryDriverPhone,
            EstimatedDistance = shipment.EstimatedDistance,
            EstimatedDuration = shipment.EstimatedDuration,
            CreatedAt = shipment.CreatedAt
        };
    }
}

public class GetShipmentByTrackingNumberQueryHandler : IRequestHandler<GetShipmentByTrackingNumberQuery, ShipmentDto?>
{
    private readonly ShippingDbContext _context;

    public GetShipmentByTrackingNumberQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentDto?> Handle(GetShipmentByTrackingNumberQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _context.Shipments
            .Include(s => s.ShippingMethod)
            .FirstOrDefaultAsync(s => s.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (shipment == null)
            return null;

        return new ShipmentDto
        {
            Id = shipment.Id,
            OrderId = shipment.OrderId,
            CustomerId = shipment.CustomerId,
            TrackingNumber = shipment.TrackingNumber ?? string.Empty,
            Status = shipment.Status,
            OriginAddress = shipment.OriginAddress,
            DestinationAddress = shipment.DestinationAddress,
            Weight = shipment.Weight,
            TotalCost = shipment.TotalCost,
            EstimatedDeliveryDate = shipment.EstimatedDeliveryDate,
            ActualDeliveryDate = shipment.ActualDeliveryDate,
            DeliveryDriverName = shipment.DeliveryDriverName,
            DeliveryDriverPhone = shipment.DeliveryDriverPhone,
            EstimatedDistance = shipment.EstimatedDistance,
            EstimatedDuration = shipment.EstimatedDuration,
            CreatedAt = shipment.CreatedAt
        };
    }
}

public class GetShipmentsByCustomerIdQueryHandler : IRequestHandler<GetShipmentsByCustomerIdQuery, IEnumerable<ShipmentDto>>
{
    private readonly ShippingDbContext _context;

    public GetShipmentsByCustomerIdQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShipmentDto>> Handle(GetShipmentsByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var shipments = await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.CustomerId == request.CustomerId)
            .Select(s => new ShipmentDto
            {
                Id = s.Id,
                OrderId = s.OrderId,
                CustomerId = s.CustomerId,
                TrackingNumber = s.TrackingNumber ?? string.Empty,
                Status = s.Status,
                OriginAddress = s.OriginAddress,
                DestinationAddress = s.DestinationAddress,
                Weight = s.Weight,
                TotalCost = s.TotalCost,
                EstimatedDeliveryDate = s.EstimatedDeliveryDate,
                ActualDeliveryDate = s.ActualDeliveryDate,
                DeliveryDriverName = s.DeliveryDriverName,
                DeliveryDriverPhone = s.DeliveryDriverPhone,
                EstimatedDistance = s.EstimatedDistance,
                EstimatedDuration = s.EstimatedDuration,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return shipments;
    }
}

public class GetShipmentsByDriverIdQueryHandler : IRequestHandler<GetShipmentsByDriverIdQuery, IEnumerable<ShipmentDto>>
{
    private readonly ShippingDbContext _context;

    public GetShipmentsByDriverIdQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShipmentDto>> Handle(GetShipmentsByDriverIdQuery request, CancellationToken cancellationToken)
    {
        var shipments = await _context.Shipments
            .Include(s => s.ShippingMethod)
            .Where(s => s.DeliveryDriverId == request.DriverId)
            .Select(s => new ShipmentDto
            {
                Id = s.Id,
                OrderId = s.OrderId,
                CustomerId = s.CustomerId,
                TrackingNumber = s.TrackingNumber ?? string.Empty,
                Status = s.Status,
                OriginAddress = s.OriginAddress,
                DestinationAddress = s.DestinationAddress,
                Weight = s.Weight,
                TotalCost = s.TotalCost,
                EstimatedDeliveryDate = s.EstimatedDeliveryDate,
                ActualDeliveryDate = s.ActualDeliveryDate,
                DeliveryDriverName = s.DeliveryDriverName,
                DeliveryDriverPhone = s.DeliveryDriverPhone,
                EstimatedDistance = s.EstimatedDistance,
                EstimatedDuration = s.EstimatedDuration,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return shipments;
    }
}

public class TrackShipmentQueryHandler : IRequestHandler<TrackShipmentQuery, ShipmentTrackingDto?>
{
    private readonly ShippingDbContext _context;

    public TrackShipmentQueryHandler(ShippingDbContext context)
    {
        _context = context;
    }

    public async Task<ShipmentTrackingDto?> Handle(TrackShipmentQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _context.Shipments
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.TrackingNumber == request.TrackingNumber, cancellationToken);

        if (shipment == null)
            return null;

        return new ShipmentTrackingDto
        {
            ShipmentId = shipment.Id,
            TrackingNumber = shipment.TrackingNumber ?? string.Empty,
            CurrentStatus = shipment.Status,
            EstimatedDeliveryDate = shipment.EstimatedDeliveryDate,
            ActualDeliveryDate = shipment.ActualDeliveryDate,
            TrackingHistory = shipment.TrackingHistory
                .OrderBy(t => t.CreatedAt)
                .Select(t => new TrackingEventDto
                {
                    Status = t.Status,
                    Timestamp = t.CreatedAt,
                    Location = t.Location,
                    Notes = t.Notes
                })
                .ToList()
        };
    }
}
