using Microsoft.AspNetCore.Mvc;
using MediatR;
using MarketingService.Application.Features.LandingPages.Commands.UpdateLandingPage;
using MarketingService.Application.DTOs;

namespace MarketingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LandingPagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LandingPagesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LandingPageDto>> UpdateLandingPage(
        Guid id,
        [FromBody] UpdateLandingPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLandingPageCommand(
            id,
            request.Content,
            request.MetaDescription,
            request.MetaKeywords,
            "", // Assuming CustomCss and CustomJs are optional, you can pass empty strings if not provided
            request.CustomCss,
            request.CustomJs);

        try
        {
            var landingPage = await _mediator.Send(command, cancellationToken);
            return Ok(landingPage);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public record UpdateLandingPageRequest(
    string? Content,
    string? MetaDescription,
    string? MetaKeywords,
    string? CustomCss,
    string? CustomJs);
