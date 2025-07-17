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
public class ShippingController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShippingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get available shipping options for a cart
    /// </summary>
    [HttpPost("options")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AvailableShippingOptionDto>>> GetAvailableOptions(
        [FromBody] GetAvailableShippingOptionsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all shipping methods (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<ShippingMethodDto>>> GetAllShippingMethods(
        CancellationToken cancellationToken)
    {
        var query = new GetAllShippingMethodsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get shipping method by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShippingMethodDto>> GetShippingMethod(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetShippingMethodByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new shipping method (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Guid>> CreateShippingMethod(
        [FromBody] CreateShippingMethodDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateShippingMethodCommand(
            dto.Name,
            dto.Description,
            dto.BaseCost,
            dto.RequiresTimeSlot);
            
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetShippingMethod), new { id = result }, result);
    }

    /// <summary>
    /// Update shipping method (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateShippingMethod(
        Guid id,
        [FromBody] UpdateShippingMethodDto dto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateShippingMethodCommand(id, dto.Name, dto.Description, dto.BaseCost);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete shipping method (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteShippingMethod(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteShippingMethodCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Add cost rule to shipping method (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/cost-rules")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddCostRule(
        Guid id,
        [FromBody] AddCostRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCostRuleToMethodCommand(
            id,
            request.RuleType,
            request.Value,
            request.Amount,
            request.IsPercentage);
            
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Add restriction rule to shipping method (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/restriction-rules")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> AddRestrictionRule(
        Guid id,
        [FromBody] AddRestrictionRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddRestrictionRuleToMethodCommand(id, request.RuleType, request.Value);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

// Request DTOs
public class AddCostRuleRequest
{
    public int RuleType { get; set; }
    public string Value { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPercentage { get; set; }
}

public class AddRestrictionRuleRequest
{
    public int RuleType { get; set; }
    public string Value { get; set; } = string.Empty;
}
