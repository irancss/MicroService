using MongoDB.Bson.Serialization.Attributes;

public class ScheduledDiscount
{
    [BsonElement("startDate")]
    public DateTime? StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime? EndDate { get; set; }

    [BsonElement("discountPercent")]
    public decimal DiscountPercent { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; }

    [BsonElement("fixedAmount")]
    public decimal? FixedAmount { get; set; }

    // Returns true if the discount is active at the given time
    public bool IsCurrentlyActive(DateTime now)
    {
        return IsActive &&
               (!StartDate.HasValue || StartDate.Value <= now) &&
               (!EndDate.HasValue || EndDate.Value >= now);
    }

    // Calculates the effective percentage for ordering
    public decimal GetEffectivePercentage(decimal productPrice)
    {
        if (DiscountPercent > 0)
            return DiscountPercent;
        if (FixedAmount.HasValue && productPrice > 0)
            return (FixedAmount.Value / productPrice) * 100;
        return 0;
    }
}
