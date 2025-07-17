using MediatR;

namespace ShippingService.Application.Features.FreeShippingRules.Queries
{
    /// <summary>
    /// Query to get a specific free shipping rule by ID
    /// </summary>
    public record GetRuleByIdQuery(Guid RuleId) : IRequest<DTOs.FreeShippingRuleDto?>;
}
