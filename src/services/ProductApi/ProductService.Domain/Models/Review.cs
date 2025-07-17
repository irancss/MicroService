using System.ComponentModel.DataAnnotations;
using ProductService.Domain.Common; // Required for validation attributes

// Assuming a namespace based on the file path
namespace ProductService.Domain.Models
{
    public class Review : AuditableEntity
    {

        public string Title { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot be longer than 1000 characters.")]
        public string Comment { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(500, ErrorMessage = "Admin reply cannot be longer than 500 characters.")]
        public string? AdminReply { get; set; }

        public Product Product { get; set; }

        // Parameterless constructor (useful for ORMs, deserialization)
        public Review()
        {
            // If Id is always a GUID, you could initialize it here too,
            // but often it's set by the database or upon creation logic.
            // For new instances created via the parameterized constructor, Id will be set.
        }

        // Constructor for creating a new review
        public Review(string productId, string userId, int rating, string comment)
        {
            Id = Guid.NewGuid().ToString(); // Generate a new unique ID
            ProductId = productId;
            UserId = userId;
            Rating = rating;
            Comment = comment;
            // IsApproved defaults to false (as per property initializer)
            // CreatedAt defaults to DateTime.UtcNow (as per property initializer)
        }

        public void Approve(string? adminReply = null)
        {
            IsApproved = true;
            AdminReply = adminReply;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRating(int rating, string comment)
        {
            // Consider if business logic requires re-validation or state changes here,
            // e.g., if an updated review should become unapproved.
            // For now, sticking to the original method's scope.
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
