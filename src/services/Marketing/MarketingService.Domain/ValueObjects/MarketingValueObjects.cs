namespace MarketingService.Domain.ValueObjects;

public record Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "USD") => new(0, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        
        return new Money(Amount + other.Amount, Currency);
    }
}

public record DateRange(DateTime StartDate, DateTime EndDate)
{
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
    public TimeSpan Duration => EndDate - StartDate;
}

public record SegmentCriteria(
    string Field,
    string Operator,
    string Value)
{
    public static SegmentCriteria Age(string ageOperator, int age) 
        => new("Age", ageOperator, age.ToString());
    
    public static SegmentCriteria Location(string city) 
        => new("Location", "equals", city);
    
    public static SegmentCriteria PurchaseAmount(string amountOperator, decimal amount) 
        => new("PurchaseAmount", amountOperator, amount.ToString());
}

public record CampaignMetrics(
    int Impressions,
    int Clicks,
    int Conversions,
    Money Spent,
    Money Revenue)
{
    public decimal ClickThroughRate => Impressions > 0 ? (decimal)Clicks / Impressions * 100 : 0;
    public decimal ConversionRate => Clicks > 0 ? (decimal)Conversions / Clicks * 100 : 0;
    public decimal ReturnOnAdSpend => Spent.Amount > 0 ? Revenue.Amount / Spent.Amount : 0;
}
