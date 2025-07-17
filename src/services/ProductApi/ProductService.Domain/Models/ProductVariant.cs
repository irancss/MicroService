using ProductService.Domain.Common;

namespace ProductService.Domain.Models;

public class ProductVariant : AuditableEntity
{
    public string ProductId { get; set; }
    public virtual Product Product { get; set; } = null!; // Required navigation to parent product

    public string Name { get; set; } = string.Empty; // e.g., "T-Shirt - Red, XL" (often auto-generated)
    public string? Sku { get; set; } // Variant-specific SKU, should be unique
    public decimal? PriceOverwrite { get; set; } // If set, overrides Product.BasePrice for this variant
    public decimal EffectivePrice => PriceOverwrite ?? Product.BasePrice;

    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; } = true; // Whether this variant is available for sale
    public int? DisplayOrder { get; set; } // Order among other variants of the same product
    public decimal? Weight { get; set; } // Variant specific weight
    public string? Gtin { get; set; } // Global Trade Item Number (e.g., UPC, EAN)


    // Navigation properties
    // Attribute values that define this variant (e.g., Color=Red, Size=XL)
    public virtual ICollection<ProductAttributeValue> DefiningAttributes { get; set; } = new List<ProductAttributeValue>();
    public virtual ICollection<ProductVariantImage> Images { get; set; } = new List<ProductVariantImage>();
    public virtual ICollection<ProductVariantPrice> Prices { get; set; } = new List<ProductVariantPrice>(); // Allows multiple price points (e.g., tiered, sale)
    public virtual ICollection<ProductVariantStock> Stocks { get; set; } = new List<ProductVariantStock>();
}