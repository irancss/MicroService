using ProductService.Domain.Common;

namespace ProductService.Domain.Models;

public class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; } // URL-friendly identifier
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}