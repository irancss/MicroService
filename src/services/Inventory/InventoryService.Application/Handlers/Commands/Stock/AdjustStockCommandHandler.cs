using MediatR;
using InventoryService.Application.Commands.Stock;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Commands.Stock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, AdjustStockResult>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly IStockAlertService _alertService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdjustStockCommandHandler> _logger;

    public AdjustStockCommandHandler(
        IProductStockRepository productStockRepository,
        IStockTransactionRepository transactionRepository,
        IStockAlertService alertService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        ILogger<AdjustStockCommandHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _transactionRepository = transactionRepository;
        _alertService = alertService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AdjustStockResult> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            
            if (productStock == null)
            {
                productStock = new ProductStock
                {
                    ProductId = request.ProductId,
                    AvailableQuantity = Math.Max(0, request.Quantity),
                    ReservedQuantity = 0
                };
                productStock = await _productStockRepository.CreateAsync(productStock);
            }
            else
            {
                var newQuantity = productStock.AvailableQuantity + request.Quantity;
                if (newQuantity < 0)
                {
                    return new AdjustStockResult
                    {
                        Success = false,
                        Message = $"Insufficient stock. Available: {productStock.AvailableQuantity}, Requested: {request.Quantity}"
                    };
                }
                
                productStock.AdjustStock(request.Quantity);
                productStock = await _productStockRepository.UpdateAsync(productStock);
            }

            // ثبت تراکنش
            var transaction = new StockTransaction
            {
                ProductId = request.ProductId,
                QuantityChange = request.Quantity,
                Type = TransactionType.Adjustment,
                Reason = request.Reason,
                UserId = request.UserId
            };
            await _transactionRepository.CreateAsync(transaction);

            await _unitOfWork.CommitTransactionAsync();

            // حذف از کش
            await _cacheService.RemoveAsync($"stock:{request.ProductId}");

            // بررسی و ارسال هشدار
            await _alertService.CheckAndPublishAlertsAsync(request.ProductId);

            _logger.LogInformation("Stock adjusted for product {ProductId}. Quantity: {Quantity}, New Available: {Available}",
                request.ProductId, request.Quantity, productStock.AvailableQuantity);

            return new AdjustStockResult
            {
                Success = true,
                Message = "Stock adjusted successfully",
                NewAvailableQuantity = productStock.AvailableQuantity,
                NewTotalQuantity = productStock.TotalQuantity,
                LowStockAlert = productStock.IsLowStock(),
                ExcessStockAlert = productStock.IsExcessStock()
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error adjusting stock for product {ProductId}", request.ProductId);
            
            return new AdjustStockResult
            {
                Success = false,
                Message = "An error occurred while adjusting stock"
            };
        }
    }
}
