namespace ShippingService.API.Models
{
    /// <summary>
    /// Request model for toggling free shipping rule status
    /// </summary>
    public class ToggleRuleStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
