using Payment.Domain.Common;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Events;

public record PaymentSucceededEvent(
    TransactionId TransactionId,
    OrderId OrderId,
    UserId UserId,
    Money Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record PaymentFailedEvent(
    TransactionId TransactionId,
    OrderId OrderId,
    UserId UserId,
    Money Amount,
    string? ErrorReason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WalletDepositedEvent(
    UserId UserId,
    Money Amount,
    Money NewBalance) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WalletWithdrawnEvent(
    UserId UserId,
    Money Amount,
    Money NewBalance) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record WalletPurchaseEvent(
    UserId UserId,
    Money Amount,
    Money NewBalance,
    string Description) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
