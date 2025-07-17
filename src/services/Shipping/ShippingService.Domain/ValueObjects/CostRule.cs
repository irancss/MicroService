using Shared.Kernel.Domain;
using ShippingService.Domain.Enums;

namespace ShippingService.Domain.ValueObjects;

public class CostRule : ValueObject
{
    public RuleType RuleType { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public bool IsPercentage { get; private set; }
    public bool IsActive { get; private set; }

    private CostRule() { } // For EF Core

    public CostRule(RuleType ruleType, string value, decimal amount, bool isPercentage = false)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or empty", nameof(value));
        
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        RuleType = ruleType;
        Value = value;
        Amount = amount;
        IsPercentage = isPercentage;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateAmount(decimal newAmount, bool isPercentage = false)
    {
        if (newAmount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(newAmount));

        Amount = newAmount;
        IsPercentage = isPercentage;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RuleType;
        yield return Value;
        yield return Amount;
        yield return IsPercentage;
    }
}
