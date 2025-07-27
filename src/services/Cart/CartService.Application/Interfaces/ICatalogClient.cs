namespace Cart.Application.Interfaces;

public interface ICatalogClient
{
    Task<ProductInfo?> GetProductInfoAsync(string productId, CancellationToken cancellationToken = default);
}

public record ProductInfo(string Id, string Name, string? ImageUrl, bool IsActive);