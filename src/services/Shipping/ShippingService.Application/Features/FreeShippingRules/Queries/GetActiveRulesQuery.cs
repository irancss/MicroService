using MediatR;

namespace ShippingService.Application.Features.FreeShippingRules.Queries
{
    /// <summary>
    /// Query to get all active free shipping rules
    /// </summary>
    public record GetActiveRulesQuery() : IRequest<IEnumerable<DTOs.FreeShippingRuleDto>>;
}
