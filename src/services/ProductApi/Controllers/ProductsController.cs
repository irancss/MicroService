using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models.Dtos;
using ProductApi.Services;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMediaService _mediaService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            IMediaService mediaService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _mediaService = mediaService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of products based on optional filters.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            if (page < 1) return BadRequest("Page must be greater than 0.");
            if (pageSize < 1) return BadRequest("PageSize must be greater than 0.");
            if (minPrice.HasValue && minPrice < 0) return BadRequest("minPrice cannot be negative.");
            if (maxPrice.HasValue && maxPrice < 0) return BadRequest("maxPrice cannot be negative.");
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest("minPrice cannot be greater than maxPrice.");

            pageSize = Math.Min(pageSize, 50);
            var result = await _productService.GetProductsAsync(page, pageSize, category, minPrice, maxPrice);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its unique ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Product ID is required.");
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID '{ProductId}' not found.", id);
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// Retrieves a specific product by its unique Sku.
        /// </summary>
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> GetProductBySku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return BadRequest("SKU is required.");
            var product = await _productService.GetProductBySkuAsync(sku);
            if (product == null)
            {
                _logger.LogWarning("Product with SKU '{Sku}' not found.", sku);
                return NotFound();
            }
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product. Requires Admin role.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");
            try
            {
                var createdProduct = await _productService.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request during product creation for SKU: {Sku}", request?.Sku);
                ModelState.AddModelError("CreateProduct", ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with SKU: {Sku}", request?.Sku);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing product. Requires Admin role.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Product ID is required.");
            if (request == null)
                return BadRequest("Request body is required.");
            try
            {
                var success = await _productService.UpdateProductAsync(id, request);
                if (!success)
                {
                    return NotFound($"Product with ID '{id}' not found for update.");
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request during product update for ID: {ProductId}", id);
                ModelState.AddModelError("UpdateProduct", ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Deletes a product (soft delete). Requires Admin role.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Product ID is required.");
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound($"Product with ID '{id}' not found for deletion.");
            }
            return NoContent();
        }

        // --- Endpoints for Media ---

        [HttpPost("{productId}/media/upload-url")]
        [ProducesResponseType(typeof(PresignedUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PresignedUrlResponse>> GetMediaUploadUrl(string productId, [FromQuery, Required] string fileName, [FromQuery, Required] string contentType)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest("Product ID is required.");
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
            {
                return BadRequest("fileName and contentType are required.");
            }

            // Optional: check if product exists
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound($"Product with ID '{productId}' not found.");

            try
            {
                var response = await _mediaService.GeneratePresignedUploadUrlAsync(productId, fileName, contentType);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating presigned URL for product {ProductId}", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate upload URL.");
            }
        }

        [HttpPost("{productId}/media/confirm")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmMediaUpload(string productId, [FromBody] MediaUploadConfirmationRequest request)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest("Product ID is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Optional: check if product exists
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return NotFound($"Product with ID '{productId}' not found.");

            try
            {
                var success = await _mediaService.ConfirmMediaUploadAsync(productId, request.S3Key, request.MediaType, request.AltText, request.Order);
                if (!success)
                {
                    return BadRequest("Failed to confirm upload and update product media.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming media upload for product {ProductId}, S3Key {S3Key}", productId, request.S3Key);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
        public class MediaUploadConfirmationRequest
        {
            [Required] public string S3Key { get; set; }
            [Required] public string MediaType { get; set; }
            public string? AltText { get; set; }
            public int Order { get; set; } = 0;
        }

        [HttpDelete("{productId}/media/{mediaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMedia(string productId, string mediaId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest("Product ID is required.");
            if (string.IsNullOrWhiteSpace(mediaId))
                return BadRequest("Media ID is required.");
            try
            {
                var success = await _mediaService.DeleteMediaAsync(productId, mediaId);
                if (!success)
                {
                    return NotFound($"Media with ID '{mediaId}' not found for product '{productId}', or deletion failed.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media for product {ProductId}, MediaId {MediaId}", productId, mediaId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
    // کنترلرهای ReviewsController و QnAController نیز به همین ترتیب با تزریق سرویس مربوطه پیاده‌سازی می‌شوند.

}
