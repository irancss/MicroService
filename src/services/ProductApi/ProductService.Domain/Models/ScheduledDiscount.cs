
using BuildingBlocks.Domain.Entities;
using ProductService.Domain.Models;

public class ScheduledDiscount : AuditableEntity<Guid>
{
    // Added an Id property, common for entities.
    public string Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // Represents the discount percentage (e.g., 10 for 10%).
    public decimal DiscountPercentage { get; set; }

    public decimal FixedDiscountAmount { get; set; }

    public bool IsActive { get; set; }

    public decimal? FixedAmount { get; set; }
    public virtual ICollection<ScheduledDiscount> ScheduledDiscounts { get; private set; } = new List<ScheduledDiscount>();
    public virtual ICollection<Question> Questions { get; private set; } = new List<Question>();
    public virtual ICollection<ProductSpecification> Specifications { get; private set; } = new List<ProductSpecification>();
    public virtual ICollection<ProductDescriptiveAttribute> DescriptiveAttributes { get; private set; } = new List<ProductDescriptiveAttribute>();
    public virtual ICollection<Review> Reviews { get; private set; } = new List<Review>();

    public string ProductId { get; set; }
    public Product Product { get; set; }

    // Default constructor
    public ScheduledDiscount()
    {
        // Sensible default: a new discount is initially active.
        IsActive = true;
    }

    // Parameterized constructor for easier initialization and basic validation.
    public ScheduledDiscount(
        DateTime? startDate,
        DateTime? endDate,
        decimal discountPercent,
        decimal? fixedAmount,
        bool isActive = true)
    {
        if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
        {
            throw new ArgumentException("StartDate cannot be after EndDate.");
        }
        if (discountPercent < 0 || discountPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "DiscountPercent must be between 0 and 100.");
        }
        if (fixedAmount.HasValue && fixedAmount.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fixedAmount), "FixedAmount cannot be negative.");
        }

        StartDate = startDate;
        EndDate = endDate;
        discountPercent = discountPercent;
        FixedAmount = fixedAmount;
        IsActive = isActive;
    }

    // Returns true if the discount is active at the given time.
    // Considers the IsActive flag and the date range.
    public bool IsCurrentlyActive(DateTime now)
    {
        return IsActive &&
               (!StartDate.HasValue || StartDate.Value <= now) &&
               (!EndDate.HasValue || EndDate.Value >= now);
    }

    // Calculates the effective percentage of the discount if it's currently active.
    // The 'now' parameter is used to determine current activity.
    public decimal GetEffectivePercentage(decimal productPrice, DateTime now)
    {
        if (!IsCurrentlyActive(now))
        {
            return 0; // Discount is not currently active.
        }

        // Prioritize percentage discount if available and positive.
        if (DiscountPercentage > 0)
        {
            return DiscountPercentage;
        }

        // If no percentage discount, consider fixed amount discount.
        if (FixedAmount.HasValue && FixedAmount.Value > 0)
        {
            // Fixed amount discount can only be expressed as a percentage if product price is positive.
            if (productPrice > 0)
            {
                return (FixedAmount.Value / productPrice) * 100;
            }
            // If product price is zero or negative, fixed amount as percentage is undefined or 0.
            return 0;
        }

        // No applicable discount.
        return 0;
    }
}
