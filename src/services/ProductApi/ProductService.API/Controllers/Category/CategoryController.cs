using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.API.Controllers.Category
{

    public class CategoryController : ApiControllerBase
    {
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger)
        {
            _logger = logger;
        }
        // CRUD operations for categories can be implemented here


        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto request)
        {
            _logger.LogInformation("Attempting to create a new category with name: {CategoryName}", request.Name);
            var command = new CreateCategoryCommand
            {
                Name = request.Name,
                Slug = request.Slug, // Assuming these are in CreateCategoryRequestDto
                Description = request.Description,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };
            var result = await Mediator.Send(command);
            _logger.LogInformation("Successfully created category with ID: {CategoryId} and Name: {CategoryName}", result, result);
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = result }, result);
        }

        [HttpGet]
        [Route("GetCategoryById/{categoryId}")]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            _logger.LogInformation("Attempting to retrieve category with ID: {CategoryId}", categoryId);
            var query = new GetCategoryByIdQuery(categoryId);
            var result = await Mediator.Send(query);
            if (result == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found", categoryId);
                return NotFound();
            }
            _logger.LogInformation("Successfully retrieved category with ID: {CategoryId}", categoryId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("Attempting to retrieve all categories");
            var query = new GetAllCategoriesQuery();
            var result = await Mediator.Send(query);
            _logger.LogInformation("Successfully retrieved all categories");
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateCategory/{categoryId}")]
        [ProducesResponseType(typeof(CategoryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateCategory(string categoryId, [FromBody] CategoryDto request)
        {
            _logger.LogInformation("Attempting to update category with ID: {CategoryId}", categoryId);
            var command = new UpdateCategoryCommand
            {
                CategoryId = categoryId,
                Name = request.Name,
                Slug = request.Slug,
                Description = request.Description,
                IsActive = request.IsActive,
                DisplayOrder = request.DisplayOrder
            };
            var result = await Mediator.Send(command);
            if (result == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found", categoryId);
                return NotFound();
            }
            _logger.LogInformation("Successfully updated category with ID: {CategoryId}", categoryId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteCategory/{categoryId}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            _logger.LogInformation("Attempting to delete category with ID: {CategoryId}", categoryId);
            var command = new DeleteCategoryCommand { CategoryId = categoryId };
            var result = await Mediator.Send(command);
            if (!result)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found", categoryId);
                return NotFound();
            }
            _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", categoryId);
            return NoContent();
        }
    }
}
