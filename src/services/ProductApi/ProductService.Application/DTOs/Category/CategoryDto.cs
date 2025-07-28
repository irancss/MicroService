
using ProductService.Domain.Models;

public class CategoryDto  
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; } // URL-friendly identifier
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string? ParentCategoryId { get; set; }
    public virtual Category? ParentCategory { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}