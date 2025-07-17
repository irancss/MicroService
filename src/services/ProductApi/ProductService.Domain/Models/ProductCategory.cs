namespace ProductService.Domain.Models;

public class ProductCategory
{
    public string ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public string CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;

    // public bool IsFeaturedProductInCategory { get; set; } // Example of additional property
}