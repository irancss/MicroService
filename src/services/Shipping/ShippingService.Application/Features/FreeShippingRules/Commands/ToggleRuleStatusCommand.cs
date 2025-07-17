using MediatR;

namespace ShippingService.Application.Features.FreeShippingRules.Commands
{
    /// <summary>
    /// Command to toggle the status of a free shipping rule (active/inactive)
    /// </summary>
    public record ToggleRuleStatusCommand(
        Guid RuleId,
        bool IsActive
    ) : IRequest<ToggleRuleStatusResponse>;

    /// <summary>
    /// Response for toggling rule status
    /// </summary>
    public class ToggleRuleStatusResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public bool NewStatus { get; set; }
    }
}
