using MediatR;

namespace InventoryService.Application.Queries.Stock;

public class GetProductStockQuery : IRequest<ProductStockDto?>
{
    public string ProductId { get; set; } = string.Empty;
}

public class ProductStockDto
{
    public string ProductId { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int TotalQuantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int ExcessStockThreshold { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsExcessStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
