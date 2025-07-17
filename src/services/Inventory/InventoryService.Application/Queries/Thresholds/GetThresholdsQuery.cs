using MediatR;

namespace InventoryService.Application.Queries.Thresholds;

public class GetThresholdsQuery : IRequest<ThresholdsDto?>
{
    public string ProductId { get; set; } = string.Empty;
}

public class GetAllThresholdsQuery : IRequest<List<ThresholdsDto>>
{
}

public class GetProductsWithAlertsQuery : IRequest<List<ProductAlertDto>>
{
}

public class ThresholdsDto
{
    public string ProductId { get; set; } = string.Empty;
    public int LowStockThreshold { get; set; }
    public int ExcessStockThreshold { get; set; }
    public int CurrentStock { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsExcessStock { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class ProductAlertDto
{
    public string ProductId { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public bool HasLowStockAlert { get; set; }
    public bool HasExcessStockAlert { get; set; }
    public int LowStockThreshold { get; set; }
    public int ExcessStockThreshold { get; set; }
}
