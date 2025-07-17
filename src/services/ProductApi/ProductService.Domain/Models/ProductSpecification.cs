using ProductService.Domain.Common;

namespace ProductService.Domain.Models;

public class ProductSpecification : AuditableEntity
{
    public virtual Product Product { get; set; } = null!;

    public string ProductId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "Material", "Weight", "Dimensions"
    public string Value { get; set; } = string.Empty; // e.g., "Cotton", "200", "10x5x2 cm"

    public string SpecificationName { get; set; }
    public string SpecificationValue { get; set; }
    public string? Unit { get; set; } // e.g., "g", "cm" (optional)
    public int DisplayOrder { get; set; }
    public bool IsVisibleToCustomer { get; set; } = true;
    public string? GroupName { get; set; } // To group specifications, e.g., "Technical Details", "Dimensions"
}