using Microsoft.AspNetCore.Mvc;
using MediatR;
using MarketingService.Application.Features.UserSegments.Queries.GetUserPersonalization;
using MarketingService.Application.DTOs;

namespace MarketingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalizationController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonalizationController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserPersonalizationDto>> GetUserPersonalization(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var query = new GetUserPersonalizationQuery(userId);
        var personalization = await _mediator.Send(query, cancellationToken);
        
        return Ok(personalization);
    }
}
