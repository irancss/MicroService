using ShippingService.Domain.Enums;
using Shared.Kernel.Domain;

namespace ShippingService.Domain.Entities;

public class Shipment : AggregateRoot
{
    public string OrderId { get; private set; }
    public string CustomerId { get; private set; }
    public Guid ShippingMethodId { get; private set; }
    public ShippingMethod ShippingMethod { get; private set; } = null!;
    
    // Address Information
    public string OriginAddress { get; private set; }
    public string DestinationAddress { get; private set; }
    public string OriginCity { get; private set; }
    public string DestinationCity { get; private set; }
    public double OriginLatitude { get; private set; }
    public double OriginLongitude { get; private set; }
    public double DestinationLatitude { get; private set; }
    public double DestinationLongitude { get; private set; }
    
    // Package Information
    public decimal Weight { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public decimal Length { get; private set; }
    public decimal DeclaredValue { get; private set; }
    
    // Shipping Details
    public decimal TotalCost { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime EstimatedDeliveryDate { get; private set; }
    public DateTime? ActualDeliveryDate { get; private set; }
    public string? TrackingNumber { get; private set; }
    
    // Route Optimization
    public string? OptimizedRoute { get; private set; }
    public decimal EstimatedDistance { get; private set; }
    public TimeSpan EstimatedDuration { get; private set; }
    
    // Delivery Information
    public string? DeliveryDriverId { get; private set; }
    public string? DeliveryDriverName { get; private set; }
    public string? DeliveryDriverPhone { get; private set; }
    public string? DeliveryNotes { get; private set; }
    
    private readonly List<ShipmentTracking> _trackingHistory = new();
    public IReadOnlyCollection<ShipmentTracking> TrackingHistory => _trackingHistory.AsReadOnly();

    protected Shipment() { }

    public Shipment(
        string orderId,
        string customerId,
        Guid shippingMethodId,
        string originAddress,
        string destinationAddress,
        string originCity,
        string destinationCity,
        double originLatitude,
        double originLongitude,
        double destinationLatitude,
        double destinationLongitude,
        decimal weight,
        decimal width,
        decimal height,
        decimal length,
        decimal declaredValue,
        decimal totalCost,
        DateTime estimatedDeliveryDate)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        CustomerId = customerId;
        ShippingMethodId = shippingMethodId;
        OriginAddress = originAddress;
        DestinationAddress = destinationAddress;
        OriginCity = originCity;
        DestinationCity = destinationCity;
        OriginLatitude = originLatitude;
        OriginLongitude = originLongitude;
        DestinationLatitude = destinationLatitude;
        DestinationLongitude = destinationLongitude;
        Weight = weight;
        Width = width;
        Height = height;
        Length = length;
        DeclaredValue = declaredValue;
        TotalCost = totalCost;
        Status = ShipmentStatus.Created;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        CreatedAt = DateTime.UtcNow;
        
        GenerateTrackingNumber();
        AddTrackingEvent(ShipmentStatus.Created, "Shipment created");
    }

    public void UpdateStatus(ShipmentStatus newStatus, string? notes = null)
    {
        if (Status == newStatus) return;
        
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        
        if (newStatus == ShipmentStatus.Delivered)
        {
            ActualDeliveryDate = DateTime.UtcNow;
        }
        
        AddTrackingEvent(newStatus, notes);
    }

    public void AssignDriver(string driverId, string driverName, string driverPhone)
    {
        DeliveryDriverId = driverId;
        DeliveryDriverName = driverName;
        DeliveryDriverPhone = driverPhone;
        UpdatedAt = DateTime.UtcNow;
        
        AddTrackingEvent(Status, $"Driver assigned: {driverName}");
    }

    public void SetOptimizedRoute(string route, decimal distance, TimeSpan duration)
    {
        OptimizedRoute = route;
        EstimatedDistance = distance;
        EstimatedDuration = duration;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDeliveryNotes(string notes)
    {
        DeliveryNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    private void GenerateTrackingNumber()
    {
        TrackingNumber = $"SHP{DateTime.Now:yyyyMMdd}{Id.ToString()[..8].ToUpper()}";
    }

    private void AddTrackingEvent(ShipmentStatus status, string? notes)
    {
        var tracking = new ShipmentTracking(Id, status, notes);
        _trackingHistory.Add(tracking);
    }
}
