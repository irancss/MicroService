using BuildingBlocks.Domain.Entities;

namespace ProductService.Domain.Models;

public class ProductVariantStock : AuditableEntity
{
    public string ProductVariantId { get; set; }
    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public string? WarehouseId { get; set; } // Optional: if tracking stock in different warehouses/locations
    public int QuantityOnHand { get; set; }
    public int ReservedQuantity { get; set; } // Stock reserved for open orders
    public int AvailableQuantity => QuantityOnHand - ReservedQuantity; // Calculated property
    public int Quantity { get; set; }
    public string WarehouseLocation { get; set; }
    public int LowStockThreshold { get; set; } // For low stock warnings
    public DateTime LastStockUpdatedAt { get; set; } = DateTime.UtcNow;
    public string? LocationInWarehouse { get; set; } // e.g. Aisle 5, Shelf B
}