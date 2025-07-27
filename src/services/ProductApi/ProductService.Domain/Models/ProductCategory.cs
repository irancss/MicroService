namespace ProductService.Domain.Models;

public class ProductCategory
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    // سازنده برای EF Core
    private ProductCategory() { }

    // سازنده عمومی برای ایجاد نمونه جدید
    public ProductCategory(Guid productId, Guid categoryId)
    {
        ProductId = productId;
        CategoryId = categoryId;
    }
}