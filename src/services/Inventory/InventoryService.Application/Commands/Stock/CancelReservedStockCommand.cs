using MediatR;

namespace InventoryService.Application.Commands.Stock;

public class CancelReservedStockCommand : IRequest<CancelReservedStockResult>
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public string? UserId { get; set; }
}

public class CancelReservedStockResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public int RemainingReservedQuantity { get; set; }
}
