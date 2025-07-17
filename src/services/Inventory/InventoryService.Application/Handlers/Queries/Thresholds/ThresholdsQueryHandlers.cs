using MediatR;
using InventoryService.Application.Queries.Thresholds;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Queries.Thresholds;

public class GetThresholdsQueryHandler : IRequestHandler<GetThresholdsQuery, ThresholdsDto?>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetThresholdsQueryHandler> _logger;

    public GetThresholdsQueryHandler(
        IProductStockRepository productStockRepository,
        ICacheService cacheService,
        ILogger<GetThresholdsQueryHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ThresholdsDto?> Handle(GetThresholdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی کش
            var cacheKey = $"thresholds:{request.ProductId}";
            var cachedThresholds = await _cacheService.GetAsync<ThresholdsDto>(cacheKey);
            if (cachedThresholds != null)
            {
                return cachedThresholds;
            }

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            if (productStock == null)
            {
                return null;
            }

            var dto = new ThresholdsDto
            {
                ProductId = productStock.ProductId,
                LowStockThreshold = productStock.LowStockThreshold,
                ExcessStockThreshold = productStock.ExcessStockThreshold,
                CurrentStock = productStock.TotalQuantity,
                IsLowStock = productStock.IsLowStock(),
                IsExcessStock = productStock.IsExcessStock(),
                LastUpdated = productStock.LastUpdated
            };

            // ذخیره در کش
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving thresholds for product {ProductId}", request.ProductId);
            return null;
        }
    }
}

public class GetAllThresholdsQueryHandler : IRequestHandler<GetAllThresholdsQuery, List<ThresholdsDto>>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly ILogger<GetAllThresholdsQueryHandler> _logger;

    public GetAllThresholdsQueryHandler(
        IProductStockRepository productStockRepository,
        ILogger<GetAllThresholdsQueryHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _logger = logger;
    }

    public async Task<List<ThresholdsDto>> Handle(GetAllThresholdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // برای پروژه واقعی باید pagination اضافه شود
            var allProducts = await _productStockRepository.GetMultipleByProductIdsAsync(new List<string>());
            
            var results = allProducts.Select(p => new ThresholdsDto
            {
                ProductId = p.ProductId,
                LowStockThreshold = p.LowStockThreshold,
                ExcessStockThreshold = p.ExcessStockThreshold,
                CurrentStock = p.TotalQuantity,
                IsLowStock = p.IsLowStock(),
                IsExcessStock = p.IsExcessStock(),
                LastUpdated = p.LastUpdated
            }).ToList();

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all thresholds");
            return new List<ThresholdsDto>();
        }
    }
}

public class GetProductsWithAlertsQueryHandler : IRequestHandler<GetProductsWithAlertsQuery, List<ProductAlertDto>>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly ILogger<GetProductsWithAlertsQueryHandler> _logger;

    public GetProductsWithAlertsQueryHandler(
        IProductStockRepository productStockRepository,
        ILogger<GetProductsWithAlertsQueryHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _logger = logger;
    }

    public async Task<List<ProductAlertDto>> Handle(GetProductsWithAlertsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var lowStockProducts = await _productStockRepository.GetProductsWithLowStockAsync();
            var excessStockProducts = await _productStockRepository.GetProductsWithExcessStockAsync();

            var alertProducts = new Dictionary<string, ProductAlertDto>();

            // محصولات با موجودی کم
            foreach (var product in lowStockProducts)
            {
                alertProducts[product.ProductId] = new ProductAlertDto
                {
                    ProductId = product.ProductId,
                    CurrentStock = product.TotalQuantity,
                    HasLowStockAlert = true,
                    HasExcessStockAlert = false,
                    LowStockThreshold = product.LowStockThreshold,
                    ExcessStockThreshold = product.ExcessStockThreshold
                };
            }

            // محصولات با موجودی زیاد
            foreach (var product in excessStockProducts)
            {
                if (alertProducts.ContainsKey(product.ProductId))
                {
                    alertProducts[product.ProductId].HasExcessStockAlert = true;
                }
                else
                {
                    alertProducts[product.ProductId] = new ProductAlertDto
                    {
                        ProductId = product.ProductId,
                        CurrentStock = product.TotalQuantity,
                        HasLowStockAlert = false,
                        HasExcessStockAlert = true,
                        LowStockThreshold = product.LowStockThreshold,
                        ExcessStockThreshold = product.ExcessStockThreshold
                    };
                }
            }

            var results = alertProducts.Values.ToList();
            _logger.LogDebug("Found {Count} products with alerts", results.Count);
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products with alerts");
            return new List<ProductAlertDto>();
        }
    }
}
