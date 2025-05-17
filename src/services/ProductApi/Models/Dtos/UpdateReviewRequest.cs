namespace ProductApi.Models.Dtos
{
    public class UpdateReviewRequest
    {
        public string? Id { get; set; } // Id of the review to be updated
        public string? Comment { get; set; } // Updated comment
        public int? Rating { get; set; } // Updated rating (1-5)
        public bool? IsApproved { get; set; } // Approval status (optional)
        public DateTime? UpdatedAt { get; set; } // Timestamp of the update
        public string? AdminReply { get; set; } // Admin's reply to the review (optional)
    }
}
