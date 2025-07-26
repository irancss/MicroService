using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models
{
    public class Tag : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public string UrlSlug
        {
            get
            {
                return Name?.ToLowerInvariant().Replace(" ", "-") ?? string.Empty;
            }
        }        

        // SEO Properties for Tag Pages
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; } // e.g., comma-separated keywords

        // Navigation property for many-to-many relationship with Product
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}

