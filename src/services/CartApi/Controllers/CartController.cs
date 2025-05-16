using CartApi.Models;
using CartApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }


        // GET /api/cart/{userId}
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Cart>> GetCart(string userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart ?? new Cart(userId));
        }


        // POST /api/cart/{userId}/items
        [HttpPost("{userId}/items")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Cart>> AddItem(string userId, [FromBody] AddItemRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedCart = await _cartService.AddItemAsync(userId, request);
                return Ok(updatedCart);
            }
            catch (KeyNotFoundException knfex) // اگر محصول پیدا نشد
            {
                _logger.LogWarning(knfex, "Product not found during AddItem for user {UserId}", userId);
                return BadRequest(knfex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the item.");
            }
        }

        // PUT /api/cart/{userId}/items/{productId}
        [HttpPut("{userId}/items/{productId}")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Cart>> UpdateItem(string userId, string productId, [FromBody] UpdateItemQuantityRequest request)
        {
            // مدل UpdateItemQuantityRequest فقط شامل Quantity است
            // public class UpdateItemQuantityRequest { [Range(1, 100)] public int Quantity { get; set; } }

            try
            {
                var updatedCart = await _cartService.UpdateItemQuantityAsync(userId, productId, request.Quantity);
                return Ok(updatedCart);
            }
            catch (KeyNotFoundException knfex)
            {
                _logger.LogWarning(knfex, "Cart or Item not found during UpdateItem for user {UserId}, product {ProductId}", userId, productId);
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item quantity for user {UserId}, product {ProductId}", userId, productId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the item quantity.");
            }
        }

        // DELETE /api/cart/{userId}/items/{productId}
        [HttpDelete("{userId}/items/{productId}")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)] // سبد بروز شده را برمی‌گرداند
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Cart>> RemoveItem(string userId, string productId)
        {
            try
            {
                var updatedCart = await _cartService.RemoveItemAsync(userId, productId);
                return Ok(updatedCart); // حتی اگر آیتم وجود نداشت، سبد فعلی برگردانده می‌شود
            }
            catch (KeyNotFoundException knfex) // فقط اگر خود سبد پیدا نشود
            {
                _logger.LogWarning(knfex, "Cart not found during RemoveItem for user {UserId}", userId);
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item for user {UserId}, product {ProductId}", userId, productId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while removing the item.");
            }
        }


        // DELETE /api/cart/{userId}
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // پاسخ موفق بدون محتوا
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var deleted = await _cartService.ClearCartAsync(userId);
            // ClearCartAsync در پیاده‌سازی بالا همیشه true برمی‌گرداند اگر KeyDeleteAsync خطا ندهد
            // می‌توان بررسی کرد که آیا کلیدی برای حذف وجود داشته یا نه، اگر نیاز بود
             if(!deleted) return NotFound($"Cart for user '{userId}' not found to clear.");

            return NoContent(); // 204 No Content بهترین پاسخ برای حذف موفق است
        }

        // POST /api/cart/{userId}/discount
        [HttpPost("{userId}/discount")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // کد تخفیف نامعتبر
        [ProducesResponseType(StatusCodes.Status404NotFound)] // سبد پیدا نشد
        public async Task<ActionResult<Cart>> ApplyDiscount(string userId, [FromBody] ApplyDiscountRequest request)
        {
            // مدل ApplyDiscountRequest: public class ApplyDiscountRequest { [Required] public string DiscountCode { get; set; } }
            try
            {
                var updatedCart = await _cartService.ApplyDiscountAsync(userId, request.DiscountCode);
                return Ok(updatedCart);
            }
            catch (KeyNotFoundException knfex) // سبد پیدا نشد
            {
                _logger.LogWarning(knfex, "Cart not found during ApplyDiscount for user {UserId}", userId);
                return NotFound(knfex.Message);
            }
            catch (ArgumentException argex) // کد تخفیف نامعتبر
            {
                _logger.LogWarning(argex, "Invalid discount code '{DiscountCode}' for user {UserId}", request.DiscountCode, userId);
                return BadRequest(argex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying discount for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while applying the discount.");
            }
        }
    }

}
