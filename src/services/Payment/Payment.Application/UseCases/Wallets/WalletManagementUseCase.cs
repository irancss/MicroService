using MediatR;
using Payment.Application.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using Payment.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Payment.Application.UseCases.Wallets;

// Deposit Command
public class DepositToWalletCommand : IRequest<WalletOperationResult>
{
    public WalletDepositDto DepositDto { get; }

    public DepositToWalletCommand(WalletDepositDto depositDto)
    {
        DepositDto = depositDto;
    }
}

// Withdrawal Command
public class WithdrawFromWalletCommand : IRequest<WalletOperationResult>
{
    public WalletWithdrawalDto WithdrawalDto { get; }

    public WithdrawFromWalletCommand(WalletWithdrawalDto withdrawalDto)
    {
        WithdrawalDto = withdrawalDto;
    }
}

// Purchase Command
public class PurchaseWithWalletCommand : IRequest<WalletOperationResult>
{
    public WalletPurchaseDto PurchaseDto { get; }

    public PurchaseWithWalletCommand(WalletPurchaseDto purchaseDto)
    {
        PurchaseDto = purchaseDto;
    }
}

// Get Wallet Query
public class GetWalletQuery : IRequest<WalletDto?>
{
    public Guid UserId { get; }

    public GetWalletQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class WalletOperationResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public WalletDto? Wallet { get; set; }
}

// Deposit Handler
public class DepositToWalletHandler : IRequestHandler<DepositToWalletCommand, WalletOperationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepositToWalletHandler> _logger;

    public DepositToWalletHandler(IUnitOfWork unitOfWork, ILogger<DepositToWalletHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WalletOperationResult> Handle(DepositToWalletCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.DepositDto;
            var userId = new UserId(dto.UserId);
            var currency = Enum.Parse<Currency>(dto.Currency);
            var amount = new Money(dto.Amount, currency);

            // Get or create wallet
            var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                wallet = new Wallet(userId, currency);
                await _unitOfWork.Wallets.AddAsync(wallet);
            }

            // Deposit to wallet
            wallet.Deposit(amount, dto.Description);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Wallet deposit successful for User {UserId}, Amount: {Amount} {Currency}",
                dto.UserId, dto.Amount, dto.Currency);

            return new WalletOperationResult
            {
                IsSuccess = true,
                Wallet = MapToDto(wallet)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error depositing to wallet for User {UserId}", request.DepositDto.UserId);
            
            return new WalletOperationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId.Value,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Balance.Currency.ToString(),
            Status = wallet.Status.ToString(),
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}

// Withdrawal Handler
public class WithdrawFromWalletHandler : IRequestHandler<WithdrawFromWalletCommand, WalletOperationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WithdrawFromWalletHandler> _logger;

    public WithdrawFromWalletHandler(IUnitOfWork unitOfWork, ILogger<WithdrawFromWalletHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WalletOperationResult> Handle(WithdrawFromWalletCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.WithdrawalDto;
            var userId = new UserId(dto.UserId);
            var currency = Enum.Parse<Currency>(dto.Currency);
            var amount = new Money(dto.Amount, currency);

            // Get wallet
            var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return new WalletOperationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Wallet not found"
                };
            }

            // Withdraw from wallet
            wallet.Withdraw(amount, dto.Description);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Wallet withdrawal successful for User {UserId}, Amount: {Amount} {Currency}",
                dto.UserId, dto.Amount, dto.Currency);

            return new WalletOperationResult
            {
                IsSuccess = true,
                Wallet = MapToDto(wallet)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing from wallet for User {UserId}", request.WithdrawalDto.UserId);
            
            return new WalletOperationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId.Value,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Balance.Currency.ToString(),
            Status = wallet.Status.ToString(),
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}

// Purchase Handler
public class PurchaseWithWalletHandler : IRequestHandler<PurchaseWithWalletCommand, WalletOperationResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PurchaseWithWalletHandler> _logger;

    public PurchaseWithWalletHandler(IUnitOfWork unitOfWork, ILogger<PurchaseWithWalletHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WalletOperationResult> Handle(PurchaseWithWalletCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.PurchaseDto;
            var userId = new UserId(dto.UserId);
            var currency = Enum.Parse<Currency>(dto.Currency);
            var amount = new Money(dto.Amount, currency);

            // Get wallet
            var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return new WalletOperationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Wallet not found"
                };
            }

            // Make purchase
            wallet.Purchase(amount, dto.Description);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Wallet purchase successful for User {UserId}, Amount: {Amount} {Currency}",
                dto.UserId, dto.Amount, dto.Currency);

            return new WalletOperationResult
            {
                IsSuccess = true,
                Wallet = MapToDto(wallet)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making purchase with wallet for User {UserId}", request.PurchaseDto.UserId);
            
            return new WalletOperationResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static WalletDto MapToDto(Wallet wallet)
    {
        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId.Value,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Balance.Currency.ToString(),
            Status = wallet.Status.ToString(),
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}

// Get Wallet Handler
public class GetWalletHandler : IRequestHandler<GetWalletQuery, WalletDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletDto?> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(new UserId(request.UserId));
        
        if (wallet == null)
            return null;

        return new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId.Value,
            Balance = wallet.Balance.Amount,
            Currency = wallet.Balance.Currency.ToString(),
            Status = wallet.Status.ToString(),
            CreatedAt = wallet.CreatedAt,
            UpdatedAt = wallet.UpdatedAt
        };
    }
}
