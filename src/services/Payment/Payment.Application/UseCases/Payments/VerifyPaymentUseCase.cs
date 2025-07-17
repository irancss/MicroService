using MediatR;
using Payment.Application.DTOs;
using Payment.Domain.Interfaces;
using Payment.Domain.ValueObjects;
using Payment.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Payment.Application.UseCases.Payments;

public class VerifyPaymentCommand : IRequest<VerifyPaymentResult>
{
    public PaymentVerificationDto VerificationDto { get; }

    public VerifyPaymentCommand(PaymentVerificationDto verificationDto)
    {
        VerificationDto = verificationDto;
    }
}

public class VerifyPaymentResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ReferenceId { get; set; }
    public TransactionDto? Transaction { get; set; }
}

public class VerifyPaymentHandler : IRequestHandler<VerifyPaymentCommand, VerifyPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ILogger<VerifyPaymentHandler> _logger;

    public VerifyPaymentHandler(
        IUnitOfWork unitOfWork,
        IPaymentGatewayFactory gatewayFactory,
        ILogger<VerifyPaymentHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    public async Task<VerifyPaymentResult> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.VerificationDto;

            // Get transaction from database
            var transaction = await _unitOfWork.Transactions.GetByOrderIdAsync(new OrderId(dto.OrderId));
            if (transaction == null)
            {
                return new VerifyPaymentResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Transaction not found"
                };
            }

            // Check if transaction is in correct status for verification
            if (transaction.Status != TransactionStatus.Pending)
            {
                _logger.LogWarning("Transaction {TransactionId} is not in pending status. Current status: {Status}",
                    transaction.TransactionId.Value, transaction.Status);

                return new VerifyPaymentResult
                {
                    IsSuccess = transaction.Status == TransactionStatus.Successful,
                    ErrorMessage = transaction.Status == TransactionStatus.Successful ? 
                        "Transaction already verified" : 
                        "Transaction is not in a verifiable state",
                    Transaction = MapToDto(transaction)
                };
            }

            // Get payment gateway
            var gateway = _gatewayFactory.GetGateway(dto.GatewayName);

            // Create verification request
            var verificationRequest = new PaymentVerificationRequest
            {
                Amount = transaction.Amount.Amount,
                OrderId = dto.OrderId,
                GatewaySpecificData = dto.GatewayData
            };

            // Extract gateway transaction ID from gateway data if available
            if (dto.GatewayData.ContainsKey("Authority"))
            {
                verificationRequest.GatewayTransactionId = dto.GatewayData["Authority"];
            }
            else if (dto.GatewayData.ContainsKey("TransactionId"))
            {
                verificationRequest.GatewayTransactionId = dto.GatewayData["TransactionId"];
            }

            // Verify payment with gateway
            var gatewayResult = await gateway.VerifyPaymentAsync(verificationRequest);

            if (gatewayResult.IsSuccess)
            {
                // Mark transaction as successful
                transaction.MarkAsSuccessful(
                    verificationRequest.GatewayTransactionId,
                    gatewayResult.GatewayReferenceId,
                    gatewayResult.CardNumber);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Payment verified successfully for Order {OrderId}, Reference ID: {ReferenceId}",
                    dto.OrderId, gatewayResult.GatewayReferenceId);

                return new VerifyPaymentResult
                {
                    IsSuccess = true,
                    ReferenceId = gatewayResult.GatewayReferenceId,
                    Transaction = MapToDto(transaction)
                };
            }

            // Mark transaction as failed
            transaction.MarkAsFailed(gatewayResult.ErrorMessage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new VerifyPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = gatewayResult.ErrorMessage,
                Transaction = MapToDto(transaction)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment for Order {OrderId}", request.VerificationDto.OrderId);
            
            return new VerifyPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = "An error occurred while verifying payment"
            };
        }
    }

    private static TransactionDto MapToDto(Domain.Entities.Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            TransactionId = transaction.TransactionId.Value,
            OrderId = transaction.OrderId.Value,
            UserId = transaction.UserId.Value,
            Amount = transaction.Amount.Amount,
            Currency = transaction.Amount.Currency.ToString(),
            GatewayName = transaction.GatewayName.Value,
            Status = transaction.Status.ToString(),
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            CompletedAt = transaction.CompletedAt,
            GatewayTransactionId = transaction.GatewayTransactionId,
            GatewayReferenceId = transaction.GatewayReferenceId,
            CardNumber = transaction.CardNumber,
            Type = transaction.Type.ToString()
        };
    }
}
