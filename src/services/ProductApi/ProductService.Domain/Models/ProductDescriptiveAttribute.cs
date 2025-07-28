
using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models;

/// <summary>
/// Represents descriptive attributes assigned directly to a Product (not defining variants).
/// e.g., Product "Laptop" has DescriptiveAttribute (Attribute="Warranty", Value="1 Year").
/// </summary>
public class ProductDescriptiveAttribute : AuditableEntity<Guid>
{
    public string ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public string AttributeName { get; set; }
    public string AttributeValue { get; set; }

    public string ProductAttributeId { get; set; } // FK to ProductAttribute (e.g., "Warranty", "Material")
    public virtual ProductAttribute ProductAttribute { get; set; } = null!;

    public string Value { get; set; } = string.Empty; // e.g., "1 Year", "Recycled Aluminum"
}