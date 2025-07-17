namespace ShippingService.Application.Services;

public interface IProductServiceClient
{
    Task<ProductInfo?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductInfo>> GetProductsByIdsAsync(IEnumerable<string> productIds, CancellationToken cancellationToken = default);
}

public class ProductInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public bool IsActive { get; set; }
}
