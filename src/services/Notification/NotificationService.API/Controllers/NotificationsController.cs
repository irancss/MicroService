using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Commands;
using NotificationService.Application.Queries;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("email")]
    public async Task<ActionResult<bool>> SendEmail([FromBody] SendEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("sms")]
    public async Task<ActionResult<bool>> SendSms([FromBody] SendSmsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("queue-from-event")]
    public async Task<ActionResult<bool>> QueueFromEvent([FromBody] QueueNotificationFromEventCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("history/{userId}")]
    public async Task<ActionResult> GetHistory(
        string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetNotificationHistoryQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("log/{id}")]
    public async Task<ActionResult> GetLog(Guid id)
    {
        var query = new GetNotificationLogQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTemplate([FromBody] CreateTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTemplate), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateTemplate(Guid id, [FromBody] UpdateTemplateCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTemplate(Guid id)
    {
        var query = new GetTemplateQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllTemplates()
    {
        var query = new GetAllTemplatesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult> GetTemplateByName(string name)
    {
        var query = new GetTemplateQuery { Name = name };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }
}
