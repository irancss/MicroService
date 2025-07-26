using BuildingBlocks.Domain.Entities;


namespace ProductService.Domain.Models;

public class ProductVariantImage : AuditableEntity
{
    public string ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
        public bool IsPrimary { get; set; } // Is this the main image for the product variant?
    public bool IsPrimaryForVariant { get; set; } // Is this the main image for this specific variant?
    public int DisplayOrder { get; set; }
}