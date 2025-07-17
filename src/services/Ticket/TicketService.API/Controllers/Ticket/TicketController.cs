using Microsoft.AspNetCore.Mvc;
using TicketService.Application.CQRS.Tag.Commands;
using TicketService.Application.CQRS.Tag.Queries;
using TicketService.Application.DTOs;

namespace TicketService.API.Controllers.Ticket;

public class TicketController : ApiControllerBase
{
    private readonly ILogger<TicketController> _logger;
    public TicketController(ILogger<TicketController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(string id)
    {
        _logger.LogInformation("Fetching ticket with ID: {Id}", id);
        var ticket = await Mediator.Send(new GetTicketByIdQuery(id));
        if (ticket == null)
        {
            _logger.LogWarning("Ticket with ID: {Id} not found", id);
            return NotFound();
        }
        return Ok(ticket);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequestCommand request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid create ticket request: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Creating a new ticket with title: {Title}", request.Title);
        var ticket = await Mediator.Send(request);
        return CreatedAtAction(nameof(GetTicketById), new { id = ticket }, ticket);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTicket(string id, [FromBody] UpdateTicketRequestCommand request)
    {
        if (id != request.Id)
        {
            _logger.LogWarning("Ticket ID mismatch: {Id} does not match {RequestId}", id, request.Id);
            return BadRequest("Ticket ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid update ticket request: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Updating ticket with ID: {Id}", id);
        var updatedTicket = await Mediator.Send(request);
        if (updatedTicket == null)
        {
            _logger.LogWarning("Ticket with ID: {Id} not found for update", id);
            return NotFound();
        }
        return Ok(updatedTicket);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicket(string id)
    {
        _logger.LogInformation("Deleting ticket with ID: {Id}", id);
        var result = await Mediator.Send(new DeleteTicketCommand(id));
        if (!result)
        {
            _logger.LogWarning("Ticket with ID: {Id} not found for deletion", id);
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTickets([FromQuery] SearchTicketsRequestQuery request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid search tickets request: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Searching tickets with criteria: {@Request}", request);
        var tickets = await Mediator.Send(request);
        return Ok(tickets);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTicketsByStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            _logger.LogWarning("Invalid status provided for ticket search: {Status}", status);
            return BadRequest("Status cannot be empty.");
        }

        _logger.LogInformation("Fetching tickets with status: {Status}", status);
        var tickets = await Mediator.Send(new GetTicketsByStatusQuery(status));
        return Ok(tickets);
    }

    [HttpGet("priority/{priority}")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTicketsByPriority(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            _logger.LogWarning("Invalid priority provided for ticket search: {Priority}", priority);
            return BadRequest("Priority cannot be empty.");
        }

        _logger.LogInformation("Fetching tickets with priority: {Priority}", priority);
        var tickets = await Mediator.Send(new GetTicketsByPriorityQuery(priority));
        return Ok(tickets);
    }


    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketsByUserId(string userId)
    {
        if (userId.Equals(""))
        {
            _logger.LogWarning("Invalid user ID provided for ticket search: {UserId}", userId);
            return BadRequest("User ID must be greater than zero.");
        }

        _logger.LogInformation("Fetching tickets for user with ID: {UserId}", userId);
        var tickets = await Mediator.Send(new GetTicketsByUserIdQuery(userId));
        if (tickets == null || !tickets.Any())
        {
            _logger.LogWarning("No tickets found for user with ID: {UserId}", userId);
            return NotFound();
        }
        return Ok(tickets);
    }

    [HttpGet("tag/{tagId}")]
    [ProducesResponseType(typeof(IEnumerable<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketsByTagId(string tagId)
    {
        if (tagId.Equals(""))
        {
            _logger.LogWarning("Invalid tag ID provided for ticket search: {TagId}", tagId);
            return BadRequest("Tag ID must be greater than zero.");
        }

        _logger.LogInformation("Fetching tickets with tag ID: {TagId}", tagId);
        var tickets = await Mediator.Send(new GetTicketsByTagIdQuery(tagId));
        if (tickets == null || !tickets.Any())
        {
            _logger.LogWarning("No tickets found for tag with ID: {TagId}", tagId);
            return NotFound();
        }
        return Ok(tickets);
    }


    [HttpGet("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportTickets([FromQuery] ExportTicketsRequestQuery request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid export tickets request: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
            return BadRequest(ModelState);
        }

        _logger.LogInformation("Exporting tickets with criteria: {@Request}", request);
        var file = await Mediator.Send(request);
        if (file == null)
        {
            _logger.LogError("Failed to export tickets with criteria: {@Request}", request);
            return BadRequest("Failed to export tickets.");
        }
        // Assuming CSV export by default. Adjust content type and filename as necessary.
        // The 'file' variable is the byte array containing the file content.
        return File(file, "text/csv", "exported_tickets.csv");
    }
}
