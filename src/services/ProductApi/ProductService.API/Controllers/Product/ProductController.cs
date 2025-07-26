using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.CQRS.Product.Commands;
using ProductService.Application.CQRS.Product.Queries;
using ProductService.Application.DTOs.Product;

namespace ProductService.API.Controllers.Product
{
    public class ProductController : ApiControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto request)
        {
            _logger.LogInformation("Attempting to create a new product with name: {ProductName}", request.Name);
            var command = new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsActive = request.IsActive,
            };
            var result = await Mediator.Send(command);
            _logger.LogInformation("Successfully created product with ID: {ProductId} and Name: {ProductName}", result, result);
            return CreatedAtAction(nameof(GetProductById), new { productId = result }, result);
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProductById(string productId) // Assuming productId is string based on AuditableEntity
        {
            _logger.LogInformation("Attempting to retrieve product with ID: {ProductId}", productId);
            var query = new GetProductByIdQuery { Id = productId };
            var product = await Mediator.Send(query);

            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found.", productId);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", productId);
            return Ok(product);
        }
        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Attempting to retrieve all products");
            var result = await Mediator.Send(new GetAllProductsQuery(1, 10));
            _logger.LogInformation("Successfully retrieved all products");
            return Ok(result);
        }
        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(ProductDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] ProductDto request)
        {
            _logger.LogInformation("Attempting to update product with ID: {ProductId}", productId);
            //var command = new UpdateProductCommand
            //{
            //    Id = productId,
            //    Name = request.Name,
            //    Slug = "", // Assuming Slug is not provided in the request, you can generate it or leave it empty
            //    DisplayOrder = 10, // Assuming DisplayOrder is not provided in the request, you can set a default value or leave it empty
            //    Description = request.Description,
            //    Price = request.Price,
            //    StockQuantity = request.StockQuantity,
            //    IsActive = request.IsActive,
            //};
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = request.Name,
                Slug = "",// Assuming Slug is not provided in the request, you can generate it or leave it empty
                Price = request.Price,
                StockQuantity = request.StockQuantity, // Assuming StockQuantity is not provided in the request, you can set a default value or leave it empty
                Description = request.Description,
                IsActive = request.IsActive,
                DisplayOrder = 10, // Assuming DisplayOrder is not provided in the request, you can set a default value or leave it empty
            };
            var result = await Mediator.Send(command);

            if (result == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found.", productId);
                return NotFound();
            }

            _logger.LogInformation("Successfully updated product with ID: {ProductId}", productId);
            return Ok(result);
        }
        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            _logger.LogInformation("Attempting to delete product with ID: {ProductId}", productId);
            var command = new DeleteProductCommand { Id = productId };
            var result = await Mediator.Send(command);

            if (!result)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found or could not be deleted.", productId);
                return NotFound(new ProblemDetails { Title = "Product not found or could not be deleted." });
            }

            _logger.LogInformation("Successfully deleted product with ID: {ProductId}", productId);
            return NoContent();
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            _logger.LogInformation("Searching for products with query: {Query}", query);
            //var searchQuery = new SearchProductsQuery { Query = query };
            //var result = await Mediator.Send(searchQuery);
            _logger.LogInformation("Search completed, found {ProductCount} products.", 2);
            //return Ok(result);
            return Ok(new List<ProductDto>()); // Placeholder for actual implementation
        }

        
    }
}
