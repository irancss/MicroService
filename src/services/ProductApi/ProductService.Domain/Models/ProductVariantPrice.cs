using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models;

public class ProductVariantPrice : AuditableEntity<Guid>
{
    public string ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public decimal Price { get; set; }
    public string Currency { get; set; } = "تومان"; // Consider making this configurable
    public string? PriceListName { get; set; } // e.g., "Retail", "Sale", "VIP Tier 1"
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}