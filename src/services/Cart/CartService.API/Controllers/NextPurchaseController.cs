using BuildingBlocks.Core.Contracts;
using Cart.Application.Commands;
using Cart.Application.DTOs;
using Cart.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/v1/next-purchase")]
    [Authorize]
    public class NextPurchaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public NextPurchaseController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Gets the user's next-purchase (saved for later) cart.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(typeof(NextPurchaseCartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyNextPurchaseCart()
        {
            var query = new GetNextPurchaseCartQuery(_currentUser.UserId!);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Saves an item from the active cart to the next-purchase cart.
        /// </summary>
        [HttpPost("me/items")]
        [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveItemForLater([FromBody] SaveForLaterRequestDto request)
        {
            var command = new SaveItemForLaterCommand(_currentUser.UserId!, request.ProductId, request.VariantId);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Moves an item from the next-purchase cart back to the active cart.
        /// </summary>
        [HttpPost("me/items/move-to-cart")]
        [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CartOperationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MoveItemToActiveCart([FromBody] MoveToActiveCartRequestDto request)
        {
            var command = new MoveItemToActiveCartCommand(_currentUser.UserId!, request.ProductId, request.VariantId);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Removes an item from the next-purchase cart.
        /// </summary>
        [HttpDelete("me/items/{productId}")]
        [ProducesResponseType(typeof(NextPurchaseCartDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveItemFromNextPurchase(string productId, [FromQuery] string? variantId)
        {
            var command = new RemoveItemFromNextPurchaseCommand(_currentUser.UserId!, productId, variantId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }

    // DTOs for NextPurchaseController
    public record SaveForLaterRequestDto(string ProductId, string? VariantId);
    public record MoveToActiveCartRequestDto(string ProductId, string? VariantId);
}
