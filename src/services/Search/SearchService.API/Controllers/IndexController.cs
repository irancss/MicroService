using MediatR;
using Microsoft.AspNetCore.Mvc;
using SearchService.Application.Commands.IndexProduct;
using SearchService.Application.Commands.RemoveProduct;
using SearchService.Domain.Entities;

namespace SearchService.API.Controllers;

/// <summary>
/// Administrative controller for managing the search index
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class IndexController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexController> _logger;

    public IndexController(IMediator mediator, ILogger<IndexController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Index a product in the search engine
    /// </summary>
    /// <param name="product">Product to index</param>
    /// <returns>Indexing result</returns>
    [HttpPost("products")]
    [ProducesResponseType(typeof(IndexProductResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IndexProductResponse>> IndexProduct([FromBody] ProductDocument product)
    {
        try
        {
            var command = new IndexProductCommand { Product = product };
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Successfully indexed product: {ProductId}", product.Id);
                return Ok(result);
            }

            _logger.LogWarning("Failed to index product: {ProductId}, Error: {Error}", 
                product.Id, result.ErrorMessage);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing product: {ProductId}", product.Id);
            return StatusCode(500, new { error = "Internal server error occurred while indexing product" });
        }
    }

    /// <summary>
    /// Remove a product from the search index
    /// </summary>
    /// <param name="productId">ID of the product to remove</param>
    /// <returns>Removal result</returns>
    [HttpDelete("products/{productId}")]
    [ProducesResponseType(typeof(RemoveProductFromIndexResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<RemoveProductFromIndexResponse>> RemoveProduct([FromRoute] string productId)
    {
        try
        {
            var command = new RemoveProductFromIndexCommand { ProductId = productId };
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                _logger.LogInformation("Successfully removed product from index: {ProductId}", productId);
                return Ok(result);
            }

            _logger.LogWarning("Failed to remove product from index: {ProductId}, Error: {Error}", 
                productId, result.ErrorMessage);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from index: {ProductId}", productId);
            return StatusCode(500, new { error = "Internal server error occurred while removing product from index" });
        }
    }

    /// <summary>
    /// Bulk index multiple products
    /// </summary>
    /// <param name="products">List of products to index</param>
    /// <returns>Bulk indexing result</returns>
    [HttpPost("products/bulk")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> BulkIndexProducts([FromBody] List<ProductDocument> products)
    {
        try
        {
            if (!products.Any())
            {
                return BadRequest(new { error = "No products provided for bulk indexing" });
            }

            var tasks = products.Select(async product =>
            {
                var command = new IndexProductCommand { Product = product };
                return await _mediator.Send(command);
            });

            var results = await Task.WhenAll(tasks);
            var successCount = results.Count(r => r.Success);
            var failureCount = results.Count(r => !r.Success);

            _logger.LogInformation("Bulk indexing completed: {SuccessCount} successful, {FailureCount} failed", 
                successCount, failureCount);

            return Ok(new
            {
                total = products.Count,
                successful = successCount,
                failed = failureCount,
                results = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk indexing");
            return StatusCode(500, new { error = "Internal server error occurred during bulk indexing" });
        }
    }
}
