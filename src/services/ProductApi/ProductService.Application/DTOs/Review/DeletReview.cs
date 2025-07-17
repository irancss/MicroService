namespace ProductService.Application.DTOs.Review
{
    public class DeletReview
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt
        {
            get; set;
        }
    }
}
