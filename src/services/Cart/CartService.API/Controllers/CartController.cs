using BuildingBlocks.Core.Contracts;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // تمام اکشن‌ها نیاز به کاربر لاگین کرده دارند
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CartsController> _logger;

    public CartsController(IMediator mediator, ICurrentUserService currentUser, ILogger<CartsController> logger)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authenticated user's active shopping cart.
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCart()
    {
        var cartId = GetCartId();
        var query = new GetActiveCartQuery(cartId);
        var result = await _mediator.Send(query);
        return result is not null ? Ok(result) : Ok(CartDto.CreateEmpty(cartId, _currentUser.UserId)); // همیشه یک سبد خالی برگردان
    }

    /// <summary>
    /// Adds an item to the user's active shopping cart.
    /// </summary>
    [HttpPost("me/items")]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequestDto request)
    {
        var command = new AddItemToActiveCartCommand(
            GetCartId(),
            _currentUser.UserId!,
            request.ProductId,
            request.Quantity,
            request.VariantId);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Removes an item from the user's active shopping cart.
    /// </summary>
    [HttpDelete("me/items/{productId}")]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem(string productId, [FromQuery] string? variantId)
    {
        var command = new RemoveItemFromActiveCartCommand(GetCartId(), _currentUser.UserId!, productId, variantId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Clears all items from the user's active shopping cart.
    /// </summary>
    [HttpDelete("me")]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ClearCart()
    {
        var command = new ClearActiveCartCommand(GetCartId(), _currentUser.UserId!);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Merges a guest cart into the user's cart upon login.
    /// </summary>
    /// <remarks>The guest cart ID should be passed in the request header or body.</remarks>
    [HttpPost("me/merge")]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> MergeCart([FromBody] MergeCartRequestDto request)
    {
        var command = new MergeCartsCommand(_currentUser.UserId!, request.GuestCartId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    // متد کمکی برای دریافت شناسه سبد خرید
    // اگر کاربر لاگین کرده، شناسه سبد همان شناسه کاربر است
    // اگر کاربر مهمان است، شناسه از کوکی یا هدر خوانده می‌شود (در اینجا برای سادگی فرض بر لاگین است)
    private string GetCartId()
    {
        // در حالت لاگین کرده، بهترین شناسه، شناسه کاربر است.
        return _currentUser.UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
    /// <summary>
    /// Updates the quantity of an item in the user's active shopping cart.
    /// </summary>
    [HttpPut("me/items/{productId}")]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateItemQuantity(string productId, [FromBody] UpdateItemQuantityRequestDto request)
    {
        var command = new UpdateItemQuantityInActiveCartCommand(
            GetCartId(),
            _currentUser.UserId!,
            productId,
            request.NewQuantity,
            request.VariantId
        );
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

  
}

// DTOs for CartsController
public record AddItemRequestDto(string ProductId, int Quantity, string? VariantId);
public record MergeCartRequestDto(string GuestCartId);
public record UpdateItemQuantityRequestDto(int NewQuantity, string? VariantId);