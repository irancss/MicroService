using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Grpc.Net.Client;
using Cart.Application.Interfaces;

namespace Cart.Infrastructure.Services;

public class InventoryGrpcClient : IInventoryGrpcClient
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<InventoryGrpcClient> _logger;

    public InventoryGrpcClient(IOptions<GrpcSettings> grpcSettings, ILogger<InventoryGrpcClient> logger)
    {
        _logger = logger;
        _channel = GrpcChannel.ForAddress(grpcSettings.Value.InventoryServiceUrl);
    }

    public async Task<bool> CheckStockAvailabilityAsync(string productId, int quantity)
    {
        try
        {
            // TODO: Implement actual gRPC call to inventory service
            // For now, return true as a placeholder
            _logger.LogDebug("Checking stock for product {ProductId}, quantity {Quantity}", productId, quantity);
            
            // Simulate network call
            await Task.Delay(10);
            
            // Mock response - in real implementation, this would call the inventory service
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking stock availability for product {ProductId}", productId);
            return false;
        }
    }

    public async Task<decimal?> GetCurrentPriceAsync(string productId)
    {
        try
        {
            // TODO: Implement actual gRPC call to inventory service
            _logger.LogDebug("Getting current price for product {ProductId}", productId);
            
            // Simulate network call
            await Task.Delay(10);
            
            // Mock response - in real implementation, this would call the inventory service
            return 100.00m; // Mock price
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current price for product {ProductId}", productId);
            return null;
        }
    }

    public async Task<Dictionary<string, bool>> CheckMultipleStockAvailabilityAsync(Dictionary<string, int> productQuantities)
    {
        try
        {
            _logger.LogDebug("Checking stock for {Count} products", productQuantities.Count);
            
            // Simulate network call
            await Task.Delay(20);
            
            // Mock response - in real implementation, this would call the inventory service
            return productQuantities.ToDictionary(kv => kv.Key, kv => true);
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
            
            // Simulate network call
            await Task.Delay(20);
            
            // Mock response - in real implementation, this would call the inventory service
            return productIds.ToDictionary(id => id, id => 100.00m);
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

    public CatalogGrpcClient(IOptions<GrpcSettings> grpcSettings, ILogger<CatalogGrpcClient> logger)
    {
        _logger = logger;
        _channel = GrpcChannel.ForAddress(grpcSettings.Value.CatalogServiceUrl);
    }

    public async Task<ProductInfo?> GetProductInfoAsync(string productId)
    {
        try
        {
            _logger.LogDebug("Getting product info for {ProductId}", productId);
            
            // Simulate network call
            await Task.Delay(10);
            
            // Mock response - in real implementation, this would call the catalog service
            return new ProductInfo
            {
                Id = productId,
                Name = $"Product {productId}",
                ImageUrl = $"https://example.com/images/{productId}.jpg",
                IsActive = true,
                CategoryId = "category-1",
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
            
            // Simulate network call
            await Task.Delay(20);
            
            // Mock response - in real implementation, this would call the catalog service
            return productIds.Select(id => new ProductInfo
            {
                Id = id,
                Name = $"Product {id}",
                ImageUrl = $"https://example.com/images/{id}.jpg",
                IsActive = true,
                CategoryId = "category-1",
                BrandId = "brand-1"
            }).ToList();
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

public class GrpcSettings
{
    public string InventoryServiceUrl { get; set; } = "https://localhost:5001";
    public string CatalogServiceUrl { get; set; } = "https://localhost:5002";
}
