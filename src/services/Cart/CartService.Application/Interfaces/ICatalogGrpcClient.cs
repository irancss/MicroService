namespace Cart.Application.Interfaces;

public interface ICatalogGrpcClient
{
    Task<ProductInfo?> GetProductInfoAsync(string productId, CancellationToken cancellationToken = default);
}

public record ProductInfo(string Id, string Name, string? ImageUrl, bool IsActive);