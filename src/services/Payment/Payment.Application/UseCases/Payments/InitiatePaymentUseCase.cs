using MediatR;
using Payment.Application.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Interfaces;
using Payment.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Payment.Application.UseCases.Payments;

public class InitiatePaymentCommand : IRequest<InitiatePaymentResult>
{
    public CreateTransactionDto TransactionDto { get; }

    public InitiatePaymentCommand(CreateTransactionDto transactionDto)
    {
        TransactionDto = transactionDto;
    }
}

public class InitiatePaymentResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? PaymentUrl { get; set; }
    public Guid? TransactionId { get; set; }
}

public class InitiatePaymentHandler : IRequestHandler<InitiatePaymentCommand, InitiatePaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ILogger<InitiatePaymentHandler> _logger;

    public InitiatePaymentHandler(
        IUnitOfWork unitOfWork,
        IPaymentGatewayFactory gatewayFactory,
        ILogger<InitiatePaymentHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    public async Task<InitiatePaymentResult> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.TransactionDto;
            
            // Check if transaction with this OrderId already exists
            var existingTransaction = await _unitOfWork.Transactions.GetByOrderIdAsync(new OrderId(dto.OrderId));
            if (existingTransaction != null)
            {
                return new InitiatePaymentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Transaction with this Order ID already exists"
                };
            }

            // Create transaction entity
            var currency = Enum.Parse<Currency>(dto.Currency);
            var money = new Money(dto.Amount, currency);
            
            var transaction = new Transaction(
                TransactionId.New(),
                new OrderId(dto.OrderId),
                new UserId(dto.UserId),
                money,
                new GatewayName(dto.GatewayName),
                dto.Description,
                dto.CallbackUrl
            );

            // Save transaction to database
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get payment gateway
            var gateway = _gatewayFactory.GetGateway(dto.GatewayName);

            // Create payment request
            var paymentRequest = new PaymentRequest
            {
                Amount = dto.Amount,
                OrderId = dto.OrderId,
                Description = dto.Description,
                CallbackUrl = dto.CallbackUrl,
                Mobile = dto.Mobile,
                Email = dto.Email
            };

            // Request payment from gateway
            var gatewayResult = await gateway.RequestPaymentAsync(paymentRequest);

            if (gatewayResult.IsSuccess)
            {
                // Update transaction with gateway transaction ID
                if (!string.IsNullOrEmpty(gatewayResult.GatewayTransactionId))
                {
                    // Note: In a real implementation, you'd update the transaction entity
                    // For now, we'll just log it
                    _logger.LogInformation("Payment initiated successfully for Order {OrderId}, Gateway Transaction ID: {GatewayTransactionId}",
                        dto.OrderId, gatewayResult.GatewayTransactionId);
                }

                return new InitiatePaymentResult
                {
                    IsSuccess = true,
                    PaymentUrl = gatewayResult.PaymentUrl,
                    TransactionId = transaction.Id
                };
            }

            // Update transaction status to failed
            transaction.MarkAsFailed(gatewayResult.ErrorMessage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new InitiatePaymentResult
            {
                IsSuccess = false,
                ErrorMessage = gatewayResult.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating payment for Order {OrderId}", request.TransactionDto.OrderId);
            
            return new InitiatePaymentResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while initiating payment"
            };
        }
    }
}
