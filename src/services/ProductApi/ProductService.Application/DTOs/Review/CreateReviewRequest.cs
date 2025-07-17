namespace ProductService.Application.DTOs.Review
{
    public class CreateReviewRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Constructor
        public CreateReviewRequest(string productId, string userId, int rating, string comment)
        {
            ProductId = productId;
            UserId = userId;
            Rating = rating;
            Comment = comment;
        }
    }
}
