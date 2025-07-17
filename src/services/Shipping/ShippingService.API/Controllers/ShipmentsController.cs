using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShippingService.Application.Commands.Shipments;
using ShippingService.Application.Queries.Shipments;
using ShippingService.Domain.Enums;

namespace ShippingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new shipment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateShipmentResult>> CreateShipment([FromBody] CreateShipmentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get shipment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetShipment(Guid id)
    {
        var query = new GetShipmentByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Track shipment by tracking number
    /// </summary>
    [HttpGet("track/{trackingNumber}")]
    public async Task<ActionResult<ShipmentTrackingDto>> TrackShipment(string trackingNumber)
    {
        var query = new TrackShipmentQuery(trackingNumber);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get shipments by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipmentsByCustomer(string customerId)
    {
        var query = new GetShipmentsByCustomerIdQuery(customerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get shipments assigned to a driver
    /// </summary>
    [HttpGet("driver/{driverId}")]
    public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetShipmentsByDriver(string driverId)
    {
        var query = new GetShipmentsByDriverIdQuery(driverId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all active shipments
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetActiveShipments()
    {
        var query = new GetActiveShipmentsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update shipment status
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateShipmentStatus(Guid id, [FromBody] UpdateShipmentStatusRequest request)
    {
        var command = new UpdateShipmentStatusCommand(
            id, 
            request.Status, 
            request.Notes, 
            request.Location, 
            request.UpdatedByUserId);
            
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Assign driver to shipment
    /// </summary>
    [HttpPut("{id:guid}/assign-driver")]
    public async Task<IActionResult> AssignDriver(Guid id, [FromBody] AssignDriverRequest request)
    {
        var command = new AssignDriverCommand(id, request.DriverId, request.DriverName, request.DriverPhone);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Optimize shipment route
    /// </summary>
    [HttpPost("{id:guid}/optimize-route")]
    public async Task<ActionResult<RouteOptimizationResult>> OptimizeRoute(Guid id, [FromBody] OptimizeRouteRequest request)
    {
        var command = new OptimizeShipmentRouteCommand(id, request.AvoidTolls, request.AvoidHighways);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public class UpdateShipmentStatusRequest
{
    public ShipmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? Location { get; set; }
    public string? UpdatedByUserId { get; set; }
}

public class AssignDriverRequest
{
    public string DriverId { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
}

public class OptimizeRouteRequest
{
    public bool AvoidTolls { get; set; } = false;
    public bool AvoidHighways { get; set; } = false;
}
