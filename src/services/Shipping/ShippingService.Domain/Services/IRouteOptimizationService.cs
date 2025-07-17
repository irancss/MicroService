namespace ShippingService.Domain.Services;

public interface IRouteOptimizationService
{
    Task<RouteOptimizationResult> OptimizeRouteAsync(RouteOptimizationRequest request);
    Task<DeliveryTimeEstimate> EstimateDeliveryTimeAsync(string origin, string destination, DateTime startTime);
    Task<IEnumerable<RouteOptimizationResult>> OptimizeMultipleDeliveriesAsync(MultiDeliveryOptimizationRequest request);
}

public class RouteOptimizationRequest
{
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public double OriginLatitude { get; set; }
    public double OriginLongitude { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }
    public DateTime PreferredDeliveryTime { get; set; }
    public bool AvoidTolls { get; set; }
    public bool AvoidHighways { get; set; }
    public string VehicleType { get; set; } = "car";
}

public class MultiDeliveryOptimizationRequest
{
    public string DepotAddress { get; set; } = string.Empty;
    public double DepotLatitude { get; set; }
    public double DepotLongitude { get; set; }
    public List<DeliveryPoint> DeliveryPoints { get; set; } = new();
    public int VehicleCapacity { get; set; }
    public TimeSpan MaxWorkingHours { get; set; }
}

public class DeliveryPoint
{
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ShipmentId { get; set; } = string.Empty;
    public int Priority { get; set; } = 1;
    public TimeSpan ServiceTime { get; set; }
    public DateTime? PreferredTime { get; set; }
}

public class RouteOptimizationResult
{
    public string OptimizedRoute { get; set; } = string.Empty;
    public decimal TotalDistance { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public decimal EstimatedFuelCost { get; set; }
    public List<RouteStep> RouteSteps { get; set; } = new();
    public DateTime EstimatedArrivalTime { get; set; }
}

public class RouteStep
{
    public string Instruction { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public TimeSpan Duration { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class DeliveryTimeEstimate
{
    public TimeSpan EstimatedDuration { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public decimal Distance { get; set; }
    public string Route { get; set; } = string.Empty;
}
