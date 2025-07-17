using MediatR;

namespace InventoryService.Application.Commands.Thresholds;

public class UpdateThresholdsCommand : IRequest<UpdateThresholdsResult>
{
    public string ProductId { get; set; } = string.Empty;
    public int LowStockThreshold { get; set; }
    public int ExcessStockThreshold { get; set; }
    public string? UserId { get; set; }
}

public class UpdateThresholdsResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool LowStockAlert { get; set; }
    public bool ExcessStockAlert { get; set; }
}
