using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models;

public class ProductVariant : AuditableEntity<Guid> // بهتر است از Guid استفاده کنیم
{
    public Guid ProductId { get; private set; }
    public virtual Product Product { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public string? Sku { get; private set; }
    public decimal? PriceOverwrite { get; private set; }

    // [اصلاح شد] استفاده از Product.Price.Value
    // همچنین Product را private set کردیم تا از خارج تغییر نکند.
    public decimal EffectivePrice => PriceOverwrite ?? Product.Price.Value;

    public bool IsActive { get; private set; } = true;
    public int? DisplayOrder { get; private set; }

    // Navigation properties
    public virtual ICollection<ProductAttributeValue> DefiningAttributes { get; private set; } = new List<ProductAttributeValue>();
    public virtual ICollection<ProductVariantImage> Images { get; private set; } = new List<ProductVariantImage>();
    public virtual ICollection<ProductVariantPrice> Prices { get; private set; } = new List<ProductVariantPrice>();
    public virtual ProductVariantStock? Stock { get; private set; } // یک واریانت معمولاً یک موجودی دارد (رابطه یک به یک)

    // سازنده برای EF Core
    private ProductVariant() { }

    // سازنده برای ایجاد نمونه جدید
    public ProductVariant(Guid productId, string name, string? sku, decimal? priceOverwrite)
    {
        ProductId = productId;
        Name = name;
        Sku = sku;
        PriceOverwrite = priceOverwrite;
    }

    // متدهایی برای مدیریت موجودیت
    public void UpdateDetails(string newName, string? newSku)
    {
        Name = newName;
        Sku = newSku;
    }

    public void SetPriceOverwrite(decimal? newPrice)
    {
        if (newPrice.HasValue && newPrice.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newPrice), "Price overwrite must be a positive value.");
        }
        PriceOverwrite = newPrice;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}