using MediatR;
using SearchService.Domain.Entities;

namespace SearchService.Application.Commands.IndexProduct;

/// <summary>
/// Command to index a product in Elasticsearch
/// </summary>
public class IndexProductCommand : IRequest<IndexProductResponse>
{
    public ProductDocument Product { get; set; } = new();
}

public class IndexProductResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string ProductId { get; set; } = string.Empty;
}
