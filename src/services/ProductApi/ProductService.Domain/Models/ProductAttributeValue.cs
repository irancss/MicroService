using ProductService.Domain.Common;

namespace ProductService.Domain.Models;

/// <summary>
/// Represents a specific predefined value for a ProductAttribute.
/// e.g., For ProductAttribute "Color", a ProductAttributeValue could be "Red".
/// These values are then selected to define ProductVariants.
/// </summary>
public class ProductAttributeValue : AuditableEntity
{
    public string ProductAttributeId { get; set; } // Foreign key to ProductAttribute (e.g., "Color")

    public virtual ProductAttribute ProductAttribute { get; set; } = null!; // Navigation to parent attribute ("Color")

    public string ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; }

    public virtual ProductAttributeValue ParentAttributeValue { get; set; } // Optional: For hierarchical attributes, e.g., "Red" might have a parent "Color"


    public string Value { get; set; } = string.Empty; // The actual value, e.g., "Red", "XL", "Cotton"
    public string? NameOrLabel { get; set; } // Optional: A display label, e.g., "Fiery Red", "Extra Large"
    public string? ColorHexCode { get; set; } // Specific for color attributes, e.g. "#FF0000"
    public int DisplayOrder { get; set; }
}