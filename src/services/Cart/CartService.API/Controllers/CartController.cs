using MediatR;
using Microsoft.AspNetCore.Mvc;
using Cart.Application.Commands;
using Cart.Application.Queries;
using Cart.Application.DTOs;
using Cart.Domain.Enums;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/v1/carts")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get user's cart with both active and next-purchase items
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(
        [FromQuery] string? userId = null,
        [FromQuery] string? guestId = null,
        [FromQuery] bool validateStockAndPrices = true)
    {
        var query = new GetCartQuery
        {
            UserId = userId,
            GuestId = guestId,
            ValidateStockAndPrices = validateStockAndPrices
        };

        var result = await _mediator.Send(query);
        
        if (result == null)
        {
            return NotFound(new { message = "Cart not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get cart summary with counts and totals only
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<CartSummaryDto>> GetCartSummary(
        [FromQuery] string? userId = null,
        [FromQuery] string? guestId = null)
    {
        var query = new GetCartSummaryQuery
        {
            UserId = userId,
            GuestId = guestId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add item to active cart (with intelligent next-purchase activation)
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<CartOperationResult>> AddItemToActiveCart([FromBody] AddItemToCartRequest request)
    {
        var command = new AddItemToActiveCartCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            VariantId = request.VariantId,
            Attributes = request.Attributes ?? new Dictionary<string, string>()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Add item directly to next-purchase cart
    /// </summary>
    [HttpPost("next-purchase/items")]
    public async Task<ActionResult<CartOperationResult>> AddItemToNextPurchaseCart([FromBody] AddItemToCartRequest request)
    {
        var command = new AddItemToNextPurchaseCartCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            VariantId = request.VariantId,
            Attributes = request.Attributes ?? new Dictionary<string, string>()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Move item from active cart to next-purchase cart
    /// </summary>
    [HttpPost("items/{productId}/move-to-next-purchase")]
    public async Task<ActionResult<CartOperationResult>> MoveItemToNextPurchase(
        string productId,
        [FromBody] MoveItemRequest request)
    {
        var command = new MoveItemToNextPurchaseCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ProductId = productId,
            Quantity = request.Quantity
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Move item from next-purchase cart to active cart
    /// </summary>
    [HttpPost("items/{productId}/move-to-active")]
    public async Task<ActionResult<CartOperationResult>> MoveItemToActive(
        string productId,
        [FromBody] MoveItemRequest request)
    {
        var command = new MoveItemToActiveCartCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ProductId = productId,
            Quantity = request.Quantity
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Update item quantity in specified cart type
    /// </summary>
    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartOperationResult>> UpdateItemQuantity(
        string productId,
        [FromBody] UpdateItemQuantityRequest request)
    {
        var command = new UpdateCartItemQuantityCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ProductId = productId,
            Quantity = request.Quantity,
            CartType = request.CartType
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove item from specified cart type
    /// </summary>
    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartOperationResult>> RemoveItem(
        string productId,
        [FromQuery] string? userId = null,
        [FromQuery] string? guestId = null,
        [FromQuery] CartType cartType = CartType.Active)
    {
        var command = new RemoveItemFromCartCommand
        {
            UserId = userId,
            GuestId = guestId,
            ProductId = productId,
            CartType = cartType
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Clear all items from specified cart type
    /// </summary>
    [HttpDelete("clear")]
    public async Task<ActionResult<CartOperationResult>> ClearCart(
        [FromQuery] string? userId = null,
        [FromQuery] string? guestId = null,
        [FromQuery] CartType cartType = CartType.Active)
    {
        var command = new ClearCartCommand
        {
            UserId = userId,
            GuestId = guestId,
            CartType = cartType
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Merge guest cart with user cart upon login
    /// </summary>
    [HttpPost("merge")]
    public async Task<ActionResult<CartOperationResult>> MergeGuestCart([FromBody] MergeCartRequest request)
    {
        var command = new MergeGuestCartCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Manually activate next-purchase items (move to active cart)
    /// </summary>
    [HttpPost("activate-next-purchase")]
    public async Task<ActionResult<CartOperationResult>> ActivateNextPurchaseItems([FromBody] ActivateNextPurchaseRequest request)
    {
        var command = new ActivateNextPurchaseItemsCommand
        {
            UserId = request.UserId,
            GuestId = request.GuestId,
            ForceActivation = request.ForceActivation
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }
}

// Request DTOs
public class AddItemToCartRequest
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? VariantId { get; set; }
    public Dictionary<string, string>? Attributes { get; set; }
}

public class MoveItemRequest
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public int? Quantity { get; set; }
}

public class UpdateItemQuantityRequest
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public int Quantity { get; set; }
    public CartType CartType { get; set; } = CartType.Active;
}

public class MergeCartRequest
{
    public string UserId { get; set; } = string.Empty;
    public string GuestId { get; set; } = string.Empty;
}

public class ActivateNextPurchaseRequest
{
    public string? UserId { get; set; }
    public string? GuestId { get; set; }
    public bool ForceActivation { get; set; } = false;
}
