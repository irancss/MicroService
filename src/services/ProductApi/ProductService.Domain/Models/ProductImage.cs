using ProductService.Domain.Common;

namespace ProductService.Domain.Models;

public class ProductImage : AuditableEntity
{
    public string ProductId { get; set; } // Image for the base product
    public virtual Product Product { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; } // Important for accessibility and SEO
    public bool IsPrimary { get; set; } // Is this the main image for the product?
    public int DisplayOrder { get; set; }
}