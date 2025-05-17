using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ProductApi.Controllers
{
    /// <summary>
    /// Controller for managing product reviews.
    /// Provides CRUD operations for reviews.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ILogger<ReviewsController> _logger;
        // In-memory storage for demonstration purposes
        private static readonly List<ReviewDto> Reviews = new();

        /// <summary>
        /// Gets all reviews.
        /// </summary>
        /// <returns>List of all reviews.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<ReviewDto>> GetAll()
        {
            return Ok(Reviews);
        }

        /// <summary>
        /// Gets a review by its ID.
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <returns>The review with the specified ID.</returns>
        [HttpGet("{id}")]
        public ActionResult<ReviewDto> GetById(int id)
        {
            var review = Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound();
            return Ok(review);
        }

        /// <summary>
        /// Creates a new review.
        /// </summary>
        /// <param name="dto">Review creation data.</param>
        /// <returns>The created review.</returns>
        [HttpPost]
        public ActionResult<ReviewDto> Create([FromBody] ReviewCreateDto dto)
        {
            var newReview = new ReviewDto
            {
                Id = Reviews.Count > 0 ? Reviews.Max(r => r.Id) + 1 : 1,
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };
            Reviews.Add(newReview);
            return CreatedAtAction(nameof(GetById), new { id = newReview.Id }, newReview);
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <param name="dto">Review update data.</param>
        /// <returns>No content if successful.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ReviewUpdateDto dto)
        {
            var review = Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound();

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            return NoContent();
        }

        /// <summary>
        /// Deletes a review by its ID.
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var review = Reviews.FirstOrDefault(r => r.Id == id);
            if (review == null)
                return NotFound();

            Reviews.Remove(review);
            return NoContent();
        }
    }

    /// <summary>
    /// Data transfer object for a review.
    /// </summary>
    public class ReviewDto
    {
        /// <summary>Review ID.</summary>
        public int Id { get; set; }
        /// <summary>Product ID.</summary>
        public int ProductId { get; set; }
        /// <summary>User ID.</summary>
        public int UserId { get; set; }
        /// <summary>Rating value.</summary>
        public int Rating { get; set; }
        /// <summary>Review comment.</summary>
        public string Comment { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a review.
    /// </summary>
    public class ReviewCreateDto
    {
        /// <summary>Product ID.</summary>
        public int ProductId { get; set; }
        /// <summary>User ID.</summary>
        public int UserId { get; set; }
        /// <summary>Rating value.</summary>
        public int Rating { get; set; }
        /// <summary>Review comment.</summary>
        public string Comment { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating a review.
    /// </summary>
    public class ReviewUpdateDto
    {
        /// <summary>Rating value.</summary>
        public int Rating { get; set; }
        /// <summary>Review comment.</summary>
        public string Comment { get; set; }
    }
}
