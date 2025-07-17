using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShippingService.Application.Commands.Returns;
using ShippingService.Application.Queries.Returns;
using ShippingService.Domain.Enums;

namespace ShippingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReturnsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReturnsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a return request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateReturnRequestResult>> CreateReturnRequest([FromBody] CreateReturnRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get return by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReturnDto>> GetReturn(Guid id)
    {
        var query = new GetReturnByIdQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Track return by tracking number
    /// </summary>
    [HttpGet("track/{trackingNumber}")]
    public async Task<ActionResult<ReturnTrackingDto>> TrackReturn(string trackingNumber)
    {
        var query = new TrackReturnQuery(trackingNumber);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get returns by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ReturnDto>>> GetReturnsByCustomer(string customerId)
    {
        var query = new GetReturnsByCustomerIdQuery(customerId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get pending returns for admin review
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<ReturnDto>>> GetPendingReturns()
    {
        var query = new GetPendingReturnsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Approve a return request
    /// </summary>
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> ApproveReturn(Guid id, [FromBody] ApproveReturnRequest request)
    {
        var command = new ApproveReturnCommand(id, request.ApprovedByUserId, request.RefundAmount, request.ApprovalNotes);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Reject a return request
    /// </summary>
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> RejectReturn(Guid id, [FromBody] RejectReturnRequest request)
    {
        var command = new RejectReturnCommand(id, request.RejectedByUserId, request.Reason);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Start return collection
    /// </summary>
    [HttpPut("{id:guid}/start-collection")]
    public async Task<IActionResult> StartCollection(Guid id, [FromBody] StartCollectionRequest request)
    {
        var command = new StartReturnCollectionCommand(id, request.CollectionDate, request.Notes);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Complete return
    /// </summary>
    [HttpPut("{id:guid}/complete")]
    public async Task<IActionResult> CompleteReturn(Guid id)
    {
        var command = new CompleteReturnCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Process return refund
    /// </summary>
    [HttpPut("{id:guid}/process-refund")]
    public async Task<IActionResult> ProcessRefund(Guid id, [FromBody] ProcessRefundRequest request)
    {
        var command = new ProcessReturnRefundCommand(id, request.TransactionId);
        await _mediator.Send(command);
        return NoContent();
    }
}

public class ApproveReturnRequest
{
    public string ApprovedByUserId { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string? ApprovalNotes { get; set; }
}

public class RejectReturnRequest
{
    public string RejectedByUserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class StartCollectionRequest
{
    public DateTime CollectionDate { get; set; }
    public string? Notes { get; set; }
}

public class ProcessRefundRequest
{
    public string TransactionId { get; set; } = string.Empty;
}
