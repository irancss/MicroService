using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cart.Application.Interfaces;

namespace Cart.Infrastructure.GrpcClients;

public class GrpcSettings
{
    public string InventoryServiceUrl { get; set; } = string.Empty;
    public string CatalogServiceUrl { get; set; } = string.Empty;
}

public class InventoryGrpcClient : IInventoryGrpcClient
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<InventoryGrpcClient> _logger;

    public InventoryGrpcClient(IOptions<GrpcSettings> settings, ILogger<InventoryGrpcClient> logger)
    {
        _logger = logger;
        _channel = GrpcChannel.ForAddress(settings.Value.InventoryServiceUrl);
    }

    public async Task<bool> CheckStockAvailabilityAsync(string productId, int quantity)
    {
        try
        {
            _logger.LogDebug("Checking stock for product {ProductId}, quantity {Quantity}", productId, quantity);
            
            // TODO: Implement actual gRPC call to inventory service
            // For now, simulate the call
            await Task.Delay(50); // Simulate network call
            
            // Mock response - in real implementation, this would be a gRPC call
            var isAvailable = !string.IsNullOrEmpty(productId) && quantity > 0 && quantity <= 100;
            
            _logger.LogDebug("Stock check result for {ProductId}: {IsAvailable}", productId, isAvailable);
            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking stock for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<decimal?> GetCurrentPriceAsync(string productId)
    {
        try
        {
            _logger.LogDebug("Getting current price for product {ProductId}", productId);
            
            // TODO: Implement actual gRPC call to inventory service
            // For now, simulate the call
            await Task.Delay(30); // Simulate network call
            
            // Mock response - in real implementation, this would be a gRPC call
            var random = new Random();
            var price = (decimal)(random.NextDouble() * 1000 + 10); // Random price between 10-1010
            
            _logger.LogDebug("Current price for {ProductId}: {Price}", productId, price);
            return Math.Round(price, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting price for product {ProductId}", productId);
            return null;
        }
    }

    public async Task<Dictionary<string, bool>> CheckMultipleStockAvailabilityAsync(Dictionary<string, int> productQuantities)
    {
        try
        {
            _logger.LogDebug("Checking stock for {Count} products", productQuantities.Count);
            
            var results = new Dictionary<string, bool>();
            
            foreach (var item in productQuantities)
            {
                var isAvailable = await CheckStockAvailabilityAsync(item.Key, item.Value);
                results[item.Key] = isAvailable;
            }
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking multiple stock availability");
            return new Dictionary<string, bool>();
        }
    }

    public async Task<Dictionary<string, decimal>> GetMultiplePricesAsync(List<string> productIds)
    {
        try
        {
            _logger.LogDebug("Getting prices for {Count} products", productIds.Count);
            
            var results = new Dictionary<string, decimal>();
            
            foreach (var productId in productIds)
            {
                var price = await GetCurrentPriceAsync(productId);
                if (price.HasValue)
                {
                    results[productId] = price.Value;
                }
            }
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple prices");
            return new Dictionary<string, decimal>();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}

public class CatalogGrpcClient : ICatalogGrpcClient
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<CatalogGrpcClient> _logger;

    public CatalogGrpcClient(IOptions<GrpcSettings> settings, ILogger<CatalogGrpcClient> logger)
    {
        _logger = logger;
        _channel = GrpcChannel.ForAddress(settings.Value.CatalogServiceUrl);
    }

    public async Task<ProductInfo?> GetProductInfoAsync(string productId)
    {
        try
        {
            _logger.LogDebug("Getting product info for {ProductId}", productId);
            
            // TODO: Implement actual gRPC call to catalog service
            // For now, simulate the call
            await Task.Delay(40); // Simulate network call
            
            // Mock response - in real implementation, this would be a gRPC call
            if (string.IsNullOrEmpty(productId))
                return null;
            
            return new ProductInfo
            {
                Id = productId,
                Name = $"Product {productId}",
                Description = $"Description for product {productId}",
                ImageUrl = $"https://images.example.com/{productId}.jpg",
                IsActive = true,
                CategoryId = "cat-1",
                BrandId = "brand-1"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product info for {ProductId}", productId);
            return null;
        }
    }

    public async Task<List<ProductInfo>> GetMultipleProductInfoAsync(List<string> productIds)
    {
        try
        {
            _logger.LogDebug("Getting product info for {Count} products", productIds.Count);
            
            var results = new List<ProductInfo>();
            
            foreach (var productId in productIds)
            {
                var productInfo = await GetProductInfoAsync(productId);
                if (productInfo != null)
                {
                    results.Add(productInfo);
                }
            }
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting multiple product info");
            return new List<ProductInfo>();
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
