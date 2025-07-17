using MediatR;

namespace InventoryService.Application.Commands.Stock;

public class AdjustStockCommand : IRequest<AdjustStockResult>
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? UserId { get; set; }
}

public class AdjustStockResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int NewAvailableQuantity { get; set; }
    public int NewTotalQuantity { get; set; }
    public bool LowStockAlert { get; set; }
    public bool ExcessStockAlert { get; set; }
}
