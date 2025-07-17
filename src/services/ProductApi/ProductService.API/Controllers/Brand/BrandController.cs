using System.Net;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.CQRS.Brand.Commands;
using ProductService.Application.CQRS.Brand.Queries;
using ProductService.Application.DTOs.Brand;

namespace ProductService.API.Controllers.Brand
{

    public class BrandController(ILogger<BrandController> logger) : ApiControllerBase
    {
        private readonly ILogger<BrandController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Creates a new brand.
        /// </summary>
        /// <param name="request">The request DTO containing brand details.</param>
        /// <returns>The created brand.</returns>
        /// <response code="201">Returns the newly created brand.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BrandDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateBrand([FromBody] BrandDto request)
        {
            _logger.LogInformation("Attempting to create a new brand with name: {BrandName}", request.Name);
            var command = new CreateBrandCommand
            {
                Name = request.Name,
                Slug = request.Slug, // Assuming these are in CreateBrandRequestDto
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };

            var result = await Mediator.Send(command);
            _logger.LogInformation("Successfully created brand with ID: {BrandId} and Name: {BrandName}", result, result);
            return CreatedAtAction(nameof(GetBrandById), new { brandId = result }, result);
        }


        /// <summary>
        /// Retrieves a specific brand by its ID.
        /// </summary>
        /// <param name="brandId">The ID of the brand to retrieve.</param>
        /// <returns>The requested brand.</returns>
        /// <response code="200">Returns the requested brand.</response>
        /// <response code="404">If the brand is not found.</response>
        [HttpGet("{brandId}")]
        [ProducesResponseType(typeof(BrandDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetBrandById(string brandId) // Assuming brandId is string based on AuditableEntity
        {
            _logger.LogInformation("Attempting to retrieve brand with ID: {BrandId}", brandId);
            var query = new GetBrandByIdQuery { Id = brandId };
            var brand = await Mediator.Send(query);

            if (brand == null)
            {
                _logger.LogWarning("Brand with ID: {BrandId} not found.", brandId);
                return NotFound(new ProblemDetails { Title = "Brand not found." });
            }
            _logger.LogInformation("Successfully retrieved brand with ID: {BrandId}", brandId);
            return Ok(brand);
        }

        /// <summary>
        /// Retrieves all brands.
        /// </summary>
        /// <returns>A list of all brands.</returns>
        /// <response code="200">Returns a list of brands.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            _logger.LogInformation("Attempting to retrieve all brands.");
            var query = new GetAllBrandsQuery(); // Assuming GetAllBrandsQuery exists
            var brands = await Mediator.Send(query);
            _logger.LogInformation("Successfully retrieved {BrandCount} brands.", brands.TotalCount);
            return Ok(brands);
        }


        /// <summary>
        /// Updates an existing brand.
        /// </summary>
        /// <param name="brandId">The ID of the brand to update.</param>
        /// <param name="request">The request DTO containing updated brand details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the brand was updated successfully.</response>
        /// <response code="400">If the request is invalid or IDs do not match.</response>
        /// <response code="404">If the brand is not found.</response>
        [HttpPut("{brandId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateBrand(string brandId, [FromBody] BrandDto request)
        {
            if (brandId != request.Id) // Assuming UpdateBrandRequestDto has an Id property
            {
                _logger.LogWarning("Mismatched Brand ID in URL ({UrlBrandId}) and body ({BodyBrandId}) for UpdateBrand request.", brandId, request.Id);
                return BadRequest(new ProblemDetails { Title = "Mismatched brand ID in URL and body." });
            }

            _logger.LogInformation("Attempting to update brand with ID: {BrandId}", brandId);
            await Mediator.Send(new UpdateBrandCommand()
            {
                Id = brandId,
                Name = request.Name,
                Slug = request.Slug ?? string.Empty,
                Description = request.Description ?? string.Empty,
                LogoUrl = request.LogoUrl ?? string.Empty,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            });
            _logger.LogInformation("Successfully updated brand with ID: {BrandId}", brandId);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific brand.
        /// </summary>
        /// <param name="brandId">The ID of the brand to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the brand was deleted successfully.</response>
        /// <response code="404">If the brand is not found.</response>
        [HttpDelete("{brandId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBrand(string brandId)
        {
            _logger.LogInformation("Attempting to delete brand with ID: {BrandId}", brandId);
            var command = new DeleteBrandCommand { Id = brandId };
            await Mediator.Send(command);
            _logger.LogInformation("Successfully deleted brand with ID: {BrandId}", brandId);
            return NoContent();
        }
    }
}
