namespace Payment.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));
        
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(Currency currency) => new(0, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
            
        if (Amount < other.Amount)
            throw new InvalidOperationException("Insufficient funds");
        
        return new Money(Amount - other.Amount, Currency);
    }

    public static implicit operator decimal(Money money) => money.Amount;
    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);
}

public enum Currency
{
    IRR = 1, // Iranian Rial
    IRT = 2  // Iranian Toman (1 Toman = 10 Rial)
}

public record TransactionId
{
    public Guid Value { get; init; }

    public TransactionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Transaction ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static TransactionId New() => new(Guid.NewGuid());
    
    public static implicit operator Guid(TransactionId transactionId) => transactionId.Value;
    public static implicit operator TransactionId(Guid value) => new(value);
}

public record OrderId
{
    public string Value { get; init; }

    public OrderId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Order ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(OrderId orderId) => orderId.Value;
    public static implicit operator OrderId(string value) => new(value);
}

public record GatewayName
{
    public string Value { get; init; }

    public GatewayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Gateway name cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator string(GatewayName gatewayName) => gatewayName.Value;
    public static implicit operator GatewayName(string value) => new(value);
}

public record UserId
{
    public Guid Value { get; init; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(value));
        
        Value = value;
    }

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => new(value);
}
