using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Controllers.Product;

namespace ProductService.API.Controllers.Tag
{

    public class TagController : ApiControllerBase
    {
        private readonly ILogger<TagController> _logger;

        public TagController(ILogger<TagController> logger)
        {
            _logger = logger;
        }
        //CRUD operations for tags can be implemented here 

        [HttpGet]
        [Route("GetAllTags")]
        public IActionResult GetAllTags()
        {
            _logger.LogInformation("Fetching all tags");

            // Logic to fetch all tags from the database or service
            return Ok(new { Message = "All tags fetched successfully" });
        }

    }
}
