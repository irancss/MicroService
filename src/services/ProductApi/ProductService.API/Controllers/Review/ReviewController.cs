using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Controllers.Product;

namespace ProductService.API.Controllers.Review
{
    public class ReviewController : ApiControllerBase
    {
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ILogger<ReviewController> logger)
        {
            _logger = logger;
        }

        // CRUD operations for reviews can be implemented here
        [HttpGet]
        [Route("GetAllReviews")]
        public IActionResult GetAllReviews()
        {
            _logger.LogInformation("Fetching all reviews");
            // Logic to fetch all reviews from the database or service
            return Ok(new { Message = "All reviews fetched successfully" });
        }
    }
}
