using Payment.Domain.Common;
using Payment.Domain.ValueObjects;
using Payment.Domain.Events;

namespace Payment.Domain.Entities;

public class Transaction : BaseEntity
{
    public TransactionId TransactionId { get; private set; }
    public OrderId OrderId { get; private set; }
    public UserId UserId { get; private set; }
    public Money Amount { get; private set; }
    public GatewayName GatewayName { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string Description { get; private set; }
    public string CallbackUrl { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayReferenceId { get; private set; }
    public string? CardNumber { get; private set; }
    public TransactionType Type { get; private set; }
    
    // For refunds
    public Guid? ParentTransactionId { get; private set; }
    public Transaction? ParentTransaction { get; private set; }
    public ICollection<Transaction> RefundTransactions { get; private set; } = new List<Transaction>();

    public Transaction(
        TransactionId transactionId,
        OrderId orderId,
        UserId userId,
        Money amount,
        GatewayName gatewayName,
        string description,
        string callbackUrl,
        TransactionType type = TransactionType.Payment)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
        GatewayName = gatewayName;
        Description = description;
        CallbackUrl = callbackUrl;
        Status = TransactionStatus.Pending;
        Type = type;
    }

    // Private constructor for EF Core
    private Transaction() 
    { 
        TransactionId = default!;
        OrderId = default!;
        UserId = default!;
        Amount = default!;
        GatewayName = default!;
        Description = string.Empty;
        CallbackUrl = string.Empty;
    }

    public void MarkAsSuccessful(string? gatewayTransactionId = null, string? gatewayReferenceId = null, string? cardNumber = null)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException($"Cannot mark transaction as successful. Current status: {Status}");

        Status = TransactionStatus.Successful;
        CompletedAt = DateTime.UtcNow;
        GatewayTransactionId = gatewayTransactionId;
        GatewayReferenceId = gatewayReferenceId;
        CardNumber = cardNumber;
        MarkAsUpdated();

        AddDomainEvent(new PaymentSucceededEvent(TransactionId, OrderId, UserId, Amount));
    }

    public void MarkAsFailed(string? errorReason = null)
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidOperationException($"Cannot mark transaction as failed. Current status: {Status}");

        Status = TransactionStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(errorReason))
            Description += $" - Error: {errorReason}";
        MarkAsUpdated();

        AddDomainEvent(new PaymentFailedEvent(TransactionId, OrderId, UserId, Amount, errorReason));
    }

    public bool CanBeRefunded()
    {
        return Status == TransactionStatus.Successful && 
               Type == TransactionType.Payment &&
               !RefundTransactions.Any(r => r.Status == TransactionStatus.Successful);
    }

    public Transaction CreateRefund(Money refundAmount, string reason)
    {
        if (!CanBeRefunded())
            throw new InvalidOperationException("Transaction cannot be refunded");

        if (refundAmount.Amount > Amount.Amount)
            throw new InvalidOperationException("Refund amount cannot exceed original amount");

        var refundTransaction = new Transaction(
            TransactionId.New(),
            OrderId,
            UserId,
            refundAmount,
            GatewayName,
            $"Refund for transaction {TransactionId.Value} - {reason}",
            CallbackUrl,
            TransactionType.Refund)
        {
            ParentTransactionId = Id
        };

        RefundTransactions.Add(refundTransaction);
        return refundTransaction;
    }
}

public enum TransactionStatus
{
    Pending = 1,
    Successful = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum TransactionType
{
    Payment = 1,
    Refund = 2,
    WalletDeposit = 3,
    WalletWithdrawal = 4,
    WalletPurchase = 5
}
