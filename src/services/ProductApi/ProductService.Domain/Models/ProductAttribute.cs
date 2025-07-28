using BuildingBlocks.Domain.Entities;


namespace ProductService.Domain.Models;

/// <summary>
/// Defines an attribute type, e.g., "Color", "Size", "Material".
/// These attributes can be used to define product variants or as descriptive properties.
/// </summary>
public class ProductAttribute : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty; // e.g., "Color", "Size"
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    public string ProductId { get; set; }
    // public bool IsVariantDefining { get; set; } // Indicates if this attribute is typically used for variants

    // Collection of predefined values for this attribute (e.g., for "Color", values could be "Red", "Blue")
    public virtual ICollection<ProductAttributeValue> PossibleValues { get; set; } = new List<ProductAttributeValue>();
}