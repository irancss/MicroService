using MediatR;
using InventoryService.Application.Queries.Stock;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Queries.Stock;

public class GetProductStockQueryHandler : IRequestHandler<GetProductStockQuery, ProductStockDto?>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetProductStockQueryHandler> _logger;

    public GetProductStockQueryHandler(
        IProductStockRepository productStockRepository,
        ICacheService cacheService,
        ILogger<GetProductStockQueryHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ProductStockDto?> Handle(GetProductStockQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // بررسی کش
            var cacheKey = $"stock:{request.ProductId}";
            var cachedStock = await _cacheService.GetAsync<ProductStockDto>(cacheKey);
            if (cachedStock != null)
            {
                _logger.LogDebug("Stock data retrieved from cache for product {ProductId}", request.ProductId);
                return cachedStock;
            }

            // دریافت از دیتابیس
            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            if (productStock == null)
            {
                return null;
            }

            var dto = new ProductStockDto
            {
                ProductId = productStock.ProductId,
                AvailableQuantity = productStock.AvailableQuantity,
                ReservedQuantity = productStock.ReservedQuantity,
                TotalQuantity = productStock.TotalQuantity,
                LowStockThreshold = productStock.LowStockThreshold,
                ExcessStockThreshold = productStock.ExcessStockThreshold,
                IsLowStock = productStock.IsLowStock(),
                IsExcessStock = productStock.IsExcessStock(),
                LastUpdated = productStock.LastUpdated
            };

            // ذخیره در کش
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

            _logger.LogDebug("Stock data retrieved from database for product {ProductId}", request.ProductId);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock for product {ProductId}", request.ProductId);
            return null;
        }
    }
}

public class GetMultipleProductStocksQueryHandler : IRequestHandler<GetMultipleProductStocksQuery, List<ProductStockDto>>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetMultipleProductStocksQueryHandler> _logger;

    public GetMultipleProductStocksQueryHandler(
        IProductStockRepository productStockRepository,
        ICacheService cacheService,
        ILogger<GetMultipleProductStocksQueryHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<List<ProductStockDto>> Handle(GetMultipleProductStocksQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var results = new List<ProductStockDto>();
            var productIdsToFetch = new List<string>();

            // بررسی کش برای هر محصول
            foreach (var productId in request.ProductIds)
            {
                var cacheKey = $"stock:{productId}";
                var cachedStock = await _cacheService.GetAsync<ProductStockDto>(cacheKey);
                if (cachedStock != null)
                {
                    results.Add(cachedStock);
                }
                else
                {
                    productIdsToFetch.Add(productId);
                }
            }

            // دریافت محصولات باقی‌مانده از دیتابیس
            if (productIdsToFetch.Any())
            {
                var productStocks = await _productStockRepository.GetMultipleByProductIdsAsync(productIdsToFetch);
                
                foreach (var productStock in productStocks)
                {
                    var dto = new ProductStockDto
                    {
                        ProductId = productStock.ProductId,
                        AvailableQuantity = productStock.AvailableQuantity,
                        ReservedQuantity = productStock.ReservedQuantity,
                        TotalQuantity = productStock.TotalQuantity,
                        LowStockThreshold = productStock.LowStockThreshold,
                        ExcessStockThreshold = productStock.ExcessStockThreshold,
                        IsLowStock = productStock.IsLowStock(),
                        IsExcessStock = productStock.IsExcessStock(),
                        LastUpdated = productStock.LastUpdated
                    };

                    results.Add(dto);

                    // ذخیره در کش
                    var cacheKey = $"stock:{productStock.ProductId}";
                    await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
                }
            }

            _logger.LogDebug("Retrieved stock data for {Count} products", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving multiple product stocks");
            return new List<ProductStockDto>();
        }
    }
}

public class GetStockTransactionsQueryHandler : IRequestHandler<GetStockTransactionsQuery, List<StockTransactionDto>>
{
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly ILogger<GetStockTransactionsQueryHandler> _logger;

    public GetStockTransactionsQueryHandler(
        IStockTransactionRepository transactionRepository,
        ILogger<GetStockTransactionsQueryHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<List<StockTransactionDto>> Handle(GetStockTransactionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _transactionRepository.GetByProductIdAsync(
                request.ProductId, 
                request.PageNumber, 
                request.PageSize);

            var results = transactions.Select(t => new StockTransactionDto
            {
                Id = t.Id,
                ProductId = t.ProductId,
                QuantityChange = t.QuantityChange,
                Type = t.Type.ToString(),
                Reference = t.Reference,
                Reason = t.Reason,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId
            }).ToList();

            _logger.LogDebug("Retrieved {Count} transactions for product {ProductId}", results.Count, request.ProductId);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for product {ProductId}", request.ProductId);
            return new List<StockTransactionDto>();
        }
    }
}
