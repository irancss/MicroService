using Microsoft.AspNetCore.Mvc;
using MediatR;
using MarketingService.Application.Features.Campaigns.Commands.CreateCampaign;
using MarketingService.Application.Features.Campaigns.Queries.GetAllCampaigns;
using MarketingService.Application.Features.Campaigns.Queries.GetCampaignBySlug;
using MarketingService.Application.DTOs;

namespace MarketingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CampaignDto>>> GetAllCampaigns(CancellationToken cancellationToken)
    {
        var query = new GetAllCampaignsQuery();
        var campaigns = await _mediator.Send(query, cancellationToken);
        return Ok(campaigns);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CampaignDto>> GetCampaignBySlug(string slug, CancellationToken cancellationToken)
    {
        var query = new GetCampaignBySlugQuery(slug);
        var campaign = await _mediator.Send(query, cancellationToken);
        
        if (campaign == null)
            return NotFound($"Campaign with slug '{slug}' not found");
            
        return Ok(campaign);
    }

    [HttpPost]
    public async Task<ActionResult<CampaignDto>> CreateCampaign(
        [FromBody] CreateCampaignRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new CreateCampaignCommand(
            request.Name,
            request.Description,
            request.Slug,
            request.Type,
            request.StartDate,
            request.EndDate,
            request.BudgetAmount,
            request.BudgetCurrency,
            request.TargetSegmentIds,
            request.CreatedBy);

        var campaign = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCampaignBySlug), new { slug = campaign.Slug }, campaign);
    }
}

public record CreateCampaignRequest(
    string Name,
    string Description,
    string Slug,
    MarketingService.Domain.Enums.CampaignType Type,
    DateTime StartDate,
    DateTime EndDate,
    decimal BudgetAmount,
    string BudgetCurrency,
    List<Guid>? TargetSegmentIds,
    string CreatedBy);
