using MediatR;
using InventoryService.Application.Commands.Stock;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Commands.Stock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, ReserveStockResult>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReserveStockCommandHandler> _logger;

    public ReserveStockCommandHandler(
        IProductStockRepository productStockRepository,
        IStockTransactionRepository transactionRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        ILogger<ReserveStockCommandHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _transactionRepository = transactionRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReserveStockResult> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            
            if (productStock == null)
            {
                return new ReserveStockResult
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            if (!productStock.TryReserveStock(request.Quantity))
            {
                return new ReserveStockResult
                {
                    Success = false,
                    Message = $"Insufficient stock. Available: {productStock.AvailableQuantity}, Requested: {request.Quantity}",
                    AvailableQuantity = productStock.AvailableQuantity,
                    ReservedQuantity = productStock.ReservedQuantity
                };
            }

            await _productStockRepository.UpdateAsync(productStock);

            // ثبت تراکنش
            var transaction = new StockTransaction
            {
                ProductId = request.ProductId,
                QuantityChange = -request.Quantity,
                Type = TransactionType.Reserve,
                Reference = request.Reference,
                UserId = request.UserId
            };
            await _transactionRepository.CreateAsync(transaction);

            await _unitOfWork.CommitTransactionAsync();

            // حذف از کش
            await _cacheService.RemoveAsync($"stock:{request.ProductId}");

            _logger.LogInformation("Stock reserved for product {ProductId}. Quantity: {Quantity}, Reference: {Reference}",
                request.ProductId, request.Quantity, request.Reference);

            return new ReserveStockResult
            {
                Success = true,
                Message = "Stock reserved successfully",
                AvailableQuantity = productStock.AvailableQuantity,
                ReservedQuantity = productStock.ReservedQuantity
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error reserving stock for product {ProductId}", request.ProductId);
            
            return new ReserveStockResult
            {
                Success = false,
                Message = "An error occurred while reserving stock"
            };
        }
    }
}
