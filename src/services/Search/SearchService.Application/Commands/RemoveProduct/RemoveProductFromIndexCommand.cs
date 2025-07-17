using MediatR;

namespace SearchService.Application.Commands.RemoveProduct;

/// <summary>
/// Command to remove a product from the search index
/// </summary>
public class RemoveProductFromIndexCommand : IRequest<RemoveProductFromIndexResponse>
{
    public string ProductId { get; set; } = string.Empty;
}

public class RemoveProductFromIndexResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string ProductId { get; set; } = string.Empty;
}
