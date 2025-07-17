using MediatR;
using InventoryService.Application.Commands.Stock;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Handlers.Commands.Stock;

public class CommitReservedStockCommandHandler : IRequestHandler<CommitReservedStockCommand, CommitReservedStockResult>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CommitReservedStockCommandHandler> _logger;

    public CommitReservedStockCommandHandler(
        IProductStockRepository productStockRepository,
        IStockTransactionRepository transactionRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        ILogger<CommitReservedStockCommandHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _transactionRepository = transactionRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CommitReservedStockResult> Handle(CommitReservedStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            
            if (productStock == null)
            {
                return new CommitReservedStockResult
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            if (productStock.ReservedQuantity < request.Quantity)
            {
                return new CommitReservedStockResult
                {
                    Success = false,
                    Message = $"Insufficient reserved stock. Reserved: {productStock.ReservedQuantity}, Requested: {request.Quantity}",
                    RemainingReservedQuantity = productStock.ReservedQuantity
                };
            }

            productStock.CommitReservedStock(request.Quantity);
            await _productStockRepository.UpdateAsync(productStock);

            // ثبت تراکنش
            var transaction = new StockTransaction
            {
                ProductId = request.ProductId,
                QuantityChange = -request.Quantity,
                Type = TransactionType.Commit,
                Reference = request.Reference,
                UserId = request.UserId
            };
            await _transactionRepository.CreateAsync(transaction);

            await _unitOfWork.CommitTransactionAsync();

            // حذف از کش
            await _cacheService.RemoveAsync($"stock:{request.ProductId}");

            _logger.LogInformation("Reserved stock committed for product {ProductId}. Quantity: {Quantity}, Reference: {Reference}",
                request.ProductId, request.Quantity, request.Reference);

            return new CommitReservedStockResult
            {
                Success = true,
                Message = "Reserved stock committed successfully",
                RemainingReservedQuantity = productStock.ReservedQuantity
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error committing reserved stock for product {ProductId}", request.ProductId);
            
            return new CommitReservedStockResult
            {
                Success = false,
                Message = "An error occurred while committing reserved stock"
            };
        }
    }
}

public class CancelReservedStockCommandHandler : IRequestHandler<CancelReservedStockCommand, CancelReservedStockResult>
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockTransactionRepository _transactionRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelReservedStockCommandHandler> _logger;

    public CancelReservedStockCommandHandler(
        IProductStockRepository productStockRepository,
        IStockTransactionRepository transactionRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        ILogger<CancelReservedStockCommandHandler> logger)
    {
        _productStockRepository = productStockRepository;
        _transactionRepository = transactionRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CancelReservedStockResult> Handle(CancelReservedStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productStock = await _productStockRepository.GetByProductIdAsync(request.ProductId);
            
            if (productStock == null)
            {
                return new CancelReservedStockResult
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            if (productStock.ReservedQuantity < request.Quantity)
            {
                return new CancelReservedStockResult
                {
                    Success = false,
                    Message = $"Insufficient reserved stock. Reserved: {productStock.ReservedQuantity}, Requested: {request.Quantity}",
                    AvailableQuantity = productStock.AvailableQuantity,
                    RemainingReservedQuantity = productStock.ReservedQuantity
                };
            }

            productStock.CancelReservedStock(request.Quantity);
            await _productStockRepository.UpdateAsync(productStock);

            // ثبت تراکنش
            var transaction = new StockTransaction
            {
                ProductId = request.ProductId,
                QuantityChange = request.Quantity,
                Type = TransactionType.Cancel,
                Reference = request.Reference,
                UserId = request.UserId
            };
            await _transactionRepository.CreateAsync(transaction);

            await _unitOfWork.CommitTransactionAsync();

            // حذف از کش
            await _cacheService.RemoveAsync($"stock:{request.ProductId}");

            _logger.LogInformation("Reserved stock cancelled for product {ProductId}. Quantity: {Quantity}, Reference: {Reference}",
                request.ProductId, request.Quantity, request.Reference);

            return new CancelReservedStockResult
            {
                Success = true,
                Message = "Reserved stock cancelled successfully",
                AvailableQuantity = productStock.AvailableQuantity,
                RemainingReservedQuantity = productStock.ReservedQuantity
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error cancelling reserved stock for product {ProductId}", request.ProductId);
            
            return new CancelReservedStockResult
            {
                Success = false,
                Message = "An error occurred while cancelling reserved stock"
            };
        }
    }
}
