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
        /// <param name="page">Page number (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 50).</param>
        /// <param name="category">Filter by category name (optional).</param>
        /// <param name="minPrice">Filter by minimum price (optional).</param>
        /// <param name="maxPrice">Filter by maximum price (optional).</param>
        /// <returns>A paginated list of products.</returns>
        /// <response code="200">Returns the paginated list of products.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            // محدود کردن pageSize برای جلوگیری از درخواست‌های سنگین
            pageSize = Math.Min(pageSize, 50); // حداکثر 50 آیتم در هر صفحه
            var result = await _productService.GetProductsAsync(page, pageSize, category, minPrice, maxPrice);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the product (MongoDB ObjectId).</param>
        /// <returns>The requested product.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
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
        /// <param name="id">The unique identifier of the product (MongoDB ObjectId).</param>
        /// <returns>The requested product.</returns>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProductBySku(string sku)
        {
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
        /// <param name="request">The details of the product to create.</param>
        /// <returns>The newly created product.</returns>
        /// <response code="201">Returns the newly created product.</response>
        /// <response code="400">If the request data is invalid (e.g., validation error, duplicate SKU).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the required Admin role.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")] // نیازمند نقش Admin
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
        {
            // ModelState.IsValid به طور خودکار توسط [ApiController] بررسی می‌شود
            try
            {
                var createdProduct = await _productService.CreateProductAsync(request);
                // بازگرداندن 201 Created به همراه آدرس محصول جدید و خود محصول
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex) // خطای مربوط به SKU تکراری یا داده نامعتبر
            {
                _logger.LogWarning(ex, "Bad request during product creation for SKU: {Sku}", request.Sku);
                // بازگرداندن جزئیات خطا به کلاینت (اختیاری)
                ModelState.AddModelError("CreateProduct", ex.Message);
                return ValidationProblem(ModelState); // 400 Bad Request با جزئیات خطا
                // یا return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with SKU: {Sku}", request.Sku);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing product. Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="request">The updated product details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Indicates successful update.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the required Admin role.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductRequest request)
        {
            // UpdateProductRequest شبیه Create است اما برخی فیلدها ممکن است Required نباشند
            try
            {
                var success = await _productService.UpdateProductAsync(id, request);
                if (!success)
                {
                    // دلیل عدم موفقیت می‌تواند NotFound یا خطای دیگری باشد که در سرویس لاگ شده
                    return NotFound($"Product with ID '{id}' not found for update.");
                }
                return NoContent(); // 204 No Content
            }
            catch (ArgumentException ex) // خطای مربوط به SKU تکراری یا داده نامعتبر
            {
                _logger.LogWarning(ex, "Bad request during product update for ID: {ProductId}", id);
                ModelState.AddModelError("UpdateProduct", ex.Message);
                return ValidationProblem(ModelState); // 400 Bad Request با جزئیات خطا
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
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">Indicates successful deletion.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user does not have the required Admin role.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var success = await _productService.DeleteProductAsync(id); // Soft delete
            if (!success)
            {
                return NotFound($"Product with ID '{id}' not found for deletion.");
            }
            return NoContent();
        }

        // --- Endpoints for Media ---

        // POST /api/products/{productId}/media/upload-url?fileName=image.jpg&contentType=image/jpeg
        [HttpPost("{productId}/media/upload-url")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PresignedUrlResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PresignedUrlResponse>> GetMediaUploadUrl(string productId, [FromQuery, Required] string fileName, [FromQuery, Required] string contentType)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(contentType))
            {
                return BadRequest("fileName and contentType are required.");
            }

            // بررسی وجود محصول (اختیاری، اما خوب است)
            // var productExists = await _productService.GetProductByIdAsync(productId) != null;
            // if(!productExists) return NotFound($"Product with ID '{productId}' not found.");

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

        // POST /api/products/{productId}/media/confirm
        [HttpPost("{productId}/media/confirm")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmMediaUpload(string productId, [FromBody] MediaUploadConfirmationRequest request)
        {
            // MediaUploadConfirmationRequest : { string S3Key, string MediaType, string AltText, int Order }
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var success = await _mediaService.ConfirmMediaUploadAsync(productId, request.S3Key, request.MediaType, request.AltText, request.Order);
                if (!success)
                {
                    // دلیل عدم موفقیت لاگ شده است (مثلا عدم یافتن محصول یا خطا در ذخیره)
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
            [Required] public string MediaType { get; set; } // "Image", "Video"
            public string? AltText { get; set; }
            public int Order { get; set; } = 0;
        }


        // DELETE /api/products/{productId}/media/{mediaId}
        [HttpDelete("{productId}/media/{mediaId}")]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedia(string productId, string mediaId)
        {
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
