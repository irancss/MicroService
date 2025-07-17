namespace ReportingService.Domain.Entities;

/// <summary>
/// Product dimension table for star schema
/// </summary>
public class ProductDimension : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string SubCategory { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private ProductDimension() { } // EF Core

    public ProductDimension(
        Guid productId, 
        string name, 
        string category, 
        string subCategory, 
        string brand, 
        decimal price, 
        string currency, 
        bool isActive = true)
    {
        ProductId = productId;
        Name = name;
        Category = category;
        SubCategory = subCategory;
        Brand = brand;
        Price = price;
        Currency = currency;
        IsActive = isActive;
    }

    public void UpdateProduct(string name, string category, string subCategory, string brand, decimal price)
    {
        Name = name;
        Category = category;
        SubCategory = subCategory;
        Brand = brand;
        Price = price;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
