using System.Text.Json;
using ShippingService.Application.Services;

namespace ShippingService.Infrastructure.Services;

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ProductInfo?> GetProductByIdAsync(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<ProductInfo>(content, _jsonOptions);
        }
        catch
        {
            // Log the exception in a real application
            return null;
        }
    }

    public async Task<IEnumerable<ProductInfo>> GetProductsByIdsAsync(IEnumerable<string> productIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var productIdList = string.Join(",", productIds);
            var response = await _httpClient.GetAsync($"api/products/batch?ids={productIdList}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ProductInfo>();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var products = JsonSerializer.Deserialize<IEnumerable<ProductInfo>>(content, _jsonOptions);
            return products ?? Enumerable.Empty<ProductInfo>();
        }
        catch
        {
            // Log the exception in a real application
            return Enumerable.Empty<ProductInfo>();
        }
    }
}
