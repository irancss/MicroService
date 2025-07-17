using MediatR;

namespace InventoryService.Application.Commands.Stock;

public class CommitReservedStockCommand : IRequest<CommitReservedStockResult>
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Reference { get; set; }
    public string? UserId { get; set; }
}

public class CommitReservedStockResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RemainingReservedQuantity { get; set; }
}
