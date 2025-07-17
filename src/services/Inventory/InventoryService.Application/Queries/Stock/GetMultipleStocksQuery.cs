using MediatR;

namespace InventoryService.Application.Queries.Stock;

public class GetMultipleProductStocksQuery : IRequest<List<ProductStockDto>>
{
    public List<string> ProductIds { get; set; } = new();
}

public class GetStockTransactionsQuery : IRequest<List<StockTransactionDto>>
{
    public string ProductId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class StockTransactionDto
{
    public Guid Id { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
}
