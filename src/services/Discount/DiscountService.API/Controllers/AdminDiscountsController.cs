using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Application.Features.Discounts.Queries;
using DiscountService.Application.DTOs;

namespace DiscountService.API.Controllers;

/// <summary>
/// Admin controller for discount management operations
/// </summary>
[ApiController]
[Route("api/admin/discounts")]
[Authorize(Roles = "Admin")]
public class AdminDiscountsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminDiscountsController> _logger;

    public AdminDiscountsController(IMediator mediator, ILogger<AdminDiscountsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new discount
    /// </summary>
    /// <param name="request">Discount creation details</param>
    /// <returns>Created discount</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] CreateDiscountRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new discount: {Name}", request.Name);

            var command = new CreateDiscountCommand
            {
                Name = request.Name,
                Description = request.Description,
                CouponCode = request.CouponCode,
                Type = request.Type,
                Value = request.Value,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsAutomatic = request.IsAutomatic,
                IsCombinableWithOthers = request.IsCombinableWithOthers,
                MaxTotalUsage = request.MaxTotalUsage,
                MaxUsagePerUser = request.MaxUsagePerUser,
                MinimumCartAmount = request.MinimumCartAmount,
                MaximumDiscountAmount = request.MaximumDiscountAmount,
                Applicability = request.Applicability,
                ApplicableProductIds = request.ApplicableProductIds,
                ApplicableCategoryIds = request.ApplicableCategoryIds,
                BuyQuantity = request.BuyQuantity,
                GetQuantity = request.GetQuantity,
                UserId = request.UserId
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Discount created successfully with ID: {Id}", result.Id);
            return CreatedAtAction(nameof(GetDiscount), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating discount: {Name}", request.Name);
            return BadRequest("Error creating discount");
        }
    }

    /// <summary>
    /// Get discount by ID
    /// </summary>
    /// <param name="id">Discount ID</param>
    /// <returns>Discount details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountDto>> GetDiscount(Guid id)
    {
        var query = new GetDiscountByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of discounts
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="searchTerm">Search term for filtering</param>
    /// <returns>Paginated discount list</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DiscountListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountListResponse>> GetDiscounts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetDiscountsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing discount
    /// </summary>
    /// <param name="id">Discount ID</param>
    /// <param name="request">Updated discount details</param>
    /// <returns>Updated discount</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountDto>> UpdateDiscount(Guid id, [FromBody] UpdateDiscountRequest request)
    {
        try
        {
            _logger.LogInformation("Updating discount: {Id}", id);

            var command = new UpdateDiscountCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                CouponCode = request.CouponCode,
                Type = request.Type,
                Value = request.Value,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsAutomatic = request.IsAutomatic,
                IsCombinableWithOthers = request.IsCombinableWithOthers,
                MaxTotalUsage = request.MaxTotalUsage,
                MaxUsagePerUser = request.MaxUsagePerUser,
                MinimumCartAmount = request.MinimumCartAmount,
                MaximumDiscountAmount = request.MaximumDiscountAmount,
                Applicability = request.Applicability,
                ApplicableProductIds = request.ApplicableProductIds,
                ApplicableCategoryIds = request.ApplicableCategoryIds,
                BuyQuantity = request.BuyQuantity,
                GetQuantity = request.GetQuantity,
                UserId = request.UserId
            };

            var result = await _mediator.Send(command);

            _logger.LogInformation("Discount updated successfully: {Id}", id);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating discount: {Id}", id);
            return BadRequest("Error updating discount");
        }
    }

    /// <summary>
    /// Delete (deactivate) a discount
    /// </summary>
    /// <param name="id">Discount ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteDiscount(Guid id)
    {
        _logger.LogInformation("Deleting discount: {Id}", id);

        var command = new DeleteDiscountCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result)
        {
            return NotFound();
        }

        _logger.LogInformation("Discount deleted successfully: {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Get discount usage history
    /// </summary>
    /// <param name="id">Discount ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Usage history list</returns>
    [HttpGet("{id:guid}/usage-history")]
    [ProducesResponseType(typeof(List<DiscountUsageHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<DiscountUsageHistoryDto>>> GetDiscountUsageHistory(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetDiscountUsageHistoryQuery
        {
            DiscountId = id,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
