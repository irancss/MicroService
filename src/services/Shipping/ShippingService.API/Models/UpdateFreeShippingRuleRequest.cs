using ShippingService.Domain.Enums;

namespace ShippingService.API.Models
{
    /// <summary>
    /// Request model for updating free shipping rule
    /// </summary>
    public class UpdateFreeShippingRuleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public bool IsPremiumOnly { get; set; }
        public int Priority { get; set; } = 1;
    }
}
