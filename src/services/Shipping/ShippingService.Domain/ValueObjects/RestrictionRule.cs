using Shared.Kernel.Domain;
using ShippingService.Domain.Enums;

namespace ShippingService.Domain.ValueObjects;

public class RestrictionRule : ValueObject
{
    public RuleType RuleType { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private RestrictionRule() { } // For EF Core

    public RestrictionRule(RuleType ruleType, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or empty", nameof(value));

        RuleType = ruleType;
        Value = value;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return RuleType;
        yield return Value;
    }
}
