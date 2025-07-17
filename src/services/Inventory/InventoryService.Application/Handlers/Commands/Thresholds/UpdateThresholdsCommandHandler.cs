using MediatR;
using InventoryService.Application.Commands.Thresholds;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Commands.Thresholds;

public class UpdateThresholdsCommandHandler : IRequestHandler<UpdateThresholdsCommand, UpdateThresholdsResult>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockAlertService _alertService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateThresholdsCommandHandler> _logger;

    public UpdateThresholdsCommandHandler(
        IProductStockRepository productStockRepository,
        IStockAlertService alertService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateThresholdsCommandHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _alertService = alertService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateThresholdsResult> Handle(UpdateThresholdsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            
            if (productStock == null)
            {
                return new UpdateThresholdsResult
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            productStock.UpdateThresholds(request.LowStockThreshold, request.ExcessStockThreshold);
            await _productStockRepository.UpdateAsync(productStock);

            await _unitOfWork.CommitTransactionAsync();

            // حذف از کش
            await _cacheService.RemoveAsync($"stock:{request.ProductId}");
            await _cacheService.RemoveAsync($"thresholds:{request.ProductId}");

            // بررسی و ارسال هشدار بعد از تغییر آستانه‌ها
            await _alertService.CheckAndPublishAlertsAsync(request.ProductId);

            _logger.LogInformation("Thresholds updated for product {ProductId}. Low: {Low}, Excess: {Excess}",
                request.ProductId, request.LowStockThreshold, request.ExcessStockThreshold);

            return new UpdateThresholdsResult
            {
                Success = true,
                Message = "Thresholds updated successfully",
                LowStockAlert = productStock.IsLowStock(),
                ExcessStockAlert = productStock.IsExcessStock()
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error updating thresholds for product {ProductId}", request.ProductId);
            
            return new UpdateThresholdsResult
            {
                Success = false,
                Message = "An error occurred while updating thresholds"
            };
        }
    }
}
