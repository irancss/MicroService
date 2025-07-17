namespace ProductService.Domain.Models;

public class ProductBrand
{
    public string ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public string BrandId { get; set; } // Changed from Guid to string
    public virtual Brand Brand { get; set; } = null!;

    // public bool IsPrimaryBrand { get; set; } // Example of additional property on join table
}