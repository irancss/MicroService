using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DiscountService.Application.Features.Discounts.Queries;
using DiscountService.Application.DTOs;

namespace DiscountService.API.Controllers;

/// <summary>
/// Controller for user-specific discount operations
/// </summary>
[ApiController]
[Route("api/users/discounts")]
[Authorize]
public class UserDiscountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserDiscountsController> _logger;

    public UserDiscountsController(IMediator mediator, ILogger<UserDiscountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's discount usage history
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>User's discount usage history</returns>
    [HttpGet("my-discount-history")]
    [ProducesResponseType(typeof(List<DiscountUsageHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DiscountUsageHistoryDto>>> GetMyDiscountHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid user ID");
        }

        var query = new GetUserDiscountHistoryQuery
        {
            UserId = userId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
