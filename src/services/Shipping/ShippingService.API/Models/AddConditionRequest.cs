using ShippingService.Domain.Enums;

namespace ShippingService.API.Controllers
{
    /// <summary>
    /// Request model for adding condition to free shipping rule
    /// </summary>
    public class AddConditionRequest
    {
        public ConditionType ConditionType { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public ComparisonOperator Operator { get; set; }
        public string Value { get; set; } = string.Empty;
        public Domain.Enums.ValueType ValueType { get; set; }
        public bool IsRequired { get; set; } = true;
        public string? Description { get; set; }
    }
}
