using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingService.Application.Commands;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries;

namespace ShippingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeSlotsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimeSlotsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get available time slots for a shipping method
    /// </summary>
    [HttpGet("shipping-method/{shippingMethodId:guid}/available")]
    public async Task<ActionResult<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(
        Guid shippingMethodId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetAvailableTimeSlotsQuery(shippingMethodId, startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create time slot template (Admin only)
    /// </summary>
    [HttpPost("templates")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Guid>> CreateTimeSlotTemplate(
        [FromBody] CreateTimeSlotTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTimeSlotTemplateCommand(
            request.ShippingMethodId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.Capacity);
            
        var result = await _mediator.Send(command, cancellationToken);
        return Created($"api/timeslots/templates/{result}", result);
    }

    /// <summary>
    /// Reserve a time slot
    /// </summary>
    [HttpPost("reserve")]
    public async Task<ActionResult<Guid>> ReserveTimeSlot(
        [FromBody] ReserveTimeSlotRequest request,
        CancellationToken cancellationToken)
    {
        var customerId = User.Identity?.Name ?? "anonymous"; // In real app, get from JWT token
        
        var command = new ReserveTimeSlotCommand(
            request.ShippingMethodId,
            request.Date,
            request.StartTime,
            request.EndTime,
            customerId,
            request.OrderId);
            
        var result = await _mediator.Send(command, cancellationToken);
        return Created($"api/timeslots/bookings/{result}", result);
    }

    /// <summary>
    /// Get time slot templates for a shipping method (Admin only)
    /// </summary>
    [HttpGet("shipping-method/{shippingMethodId:guid}/templates")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<TimeSlotTemplateDto>>> GetTimeSlotTemplates(
        Guid shippingMethodId,
        CancellationToken cancellationToken)
    {
        var query = new GetTimeSlotTemplatesByMethodQuery(shippingMethodId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

// Request DTOs
public class CreateTimeSlotTemplateRequest
{
    public Guid ShippingMethodId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }
}

public class ReserveTimeSlotRequest
{
    public Guid ShippingMethodId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? OrderId { get; set; }
}
