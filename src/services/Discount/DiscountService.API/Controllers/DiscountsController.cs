using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DiscountService.Application.Features.Discounts.Commands;
using DiscountService.Application.DTOs;

namespace DiscountService.API.Controllers;

/// <summary>
/// Controller for discount calculation operations
/// </summary>
[ApiController]
[Route("api/discounts")]
public class DiscountCalculationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DiscountCalculationController> _logger;

    public DiscountCalculationController(IMediator mediator, ILogger<DiscountCalculationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Calculate discount for a cart
    /// </summary>
    /// <param name="request">Cart details with items, user ID, shipping cost, and optional coupon code</param>
    /// <returns>Calculated discount amount and final total</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(CalculateDiscountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CalculateDiscountResponse>> CalculateDiscount([FromBody] CalculateDiscountRequest request)
    {
        try
        {
            _logger.LogInformation("Calculating discount for user {UserId} with {ItemCount} items", 
                request.UserId, request.Items.Count);

            var command = new CalculateDiscountCommand
            {
                UserId = request.UserId,
                Items = request.Items,
                ShippingCost = request.ShippingCost,
                CouponCode = request.CouponCode
            };

            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Discount calculated successfully. Amount: {Amount}", result.DiscountAmount);
            }
            else
            {
                _logger.LogWarning("Discount calculation failed: {Error}", result.ErrorMessage);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating discount for user {UserId}", request.UserId);
            return StatusCode(500, new CalculateDiscountResponse
            {
                OriginalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity) + request.ShippingCost,
                DiscountAmount = 0,
                FinalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity) + request.ShippingCost,
                IsSuccess = false,
                ErrorMessage = "An internal error occurred while calculating discount"
            });
        }
    }
}
