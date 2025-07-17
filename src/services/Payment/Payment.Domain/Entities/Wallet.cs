using Payment.Domain.Common;
using Payment.Domain.ValueObjects;
using Payment.Domain.Events;

namespace Payment.Domain.Entities;

public class Wallet : BaseEntity
{
    public UserId UserId { get; private set; }
    public Money Balance { get; private set; }
    public WalletStatus Status { get; private set; }
    
    public ICollection<WalletTransaction> Transactions { get; private set; } = new List<WalletTransaction>();

    public Wallet(UserId userId, Currency currency = Currency.IRR)
    {
        UserId = userId;
        Balance = Money.Zero(currency);
        Status = WalletStatus.Active;
    }

    // Private constructor for EF Core
    private Wallet() 
    { 
        UserId = default!;
        Balance = default!;
    }

    public void Deposit(Money amount, string description = "Wallet deposit")
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Wallet is not active");

        Balance = Balance.Add(amount);
        
        var transaction = new WalletTransaction(
            UserId,
            amount,
            WalletTransactionType.Deposit,
            description);
            
        Transactions.Add(transaction);
        MarkAsUpdated();

        AddDomainEvent(new WalletDepositedEvent(UserId, amount, Balance));
    }

    public void Withdraw(Money amount, string description = "Wallet withdrawal")
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Wallet is not active");

        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException("Insufficient balance");

        Balance = Balance.Subtract(amount);
        
        var transaction = new WalletTransaction(
            UserId,
            amount,
            WalletTransactionType.Withdrawal,
            description);
            
        Transactions.Add(transaction);
        MarkAsUpdated();

        AddDomainEvent(new WalletWithdrawnEvent(UserId, amount, Balance));
    }

    public void Purchase(Money amount, string description = "Purchase")
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Wallet is not active");

        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException("Insufficient balance for purchase");

        Balance = Balance.Subtract(amount);
        
        var transaction = new WalletTransaction(
            UserId,
            amount,
            WalletTransactionType.Purchase,
            description);
            
        Transactions.Add(transaction);
        MarkAsUpdated();

        AddDomainEvent(new WalletPurchaseEvent(UserId, amount, Balance, description));
    }

    public void Block(string reason)
    {
        Status = WalletStatus.Blocked;
        MarkAsUpdated();
    }

    public void Activate()
    {
        Status = WalletStatus.Active;
        MarkAsUpdated();
    }
}

public class WalletTransaction : BaseEntity
{
    public UserId UserId { get; private set; }
    public Money Amount { get; private set; }
    public WalletTransactionType Type { get; private set; }
    public string Description { get; private set; }

    public WalletTransaction(UserId userId, Money amount, WalletTransactionType type, string description)
    {
        UserId = userId;
        Amount = amount;
        Type = type;
        Description = description;
    }

    // Private constructor for EF Core
    private WalletTransaction() 
    { 
        UserId = default!;
        Amount = default!;
        Description = string.Empty;
    }
}

public enum WalletStatus
{
    Active = 1,
    Blocked = 2,
    Suspended = 3
}

public enum WalletTransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    Purchase = 3
}
