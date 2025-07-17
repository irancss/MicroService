using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Queries;

namespace ShippingService.Application.Features.FreeShippingRules.Queries
{
    /// <summary>
    /// Query for getting a specific free shipping rule by ID
    /// </summary>
    public class GetFreeShippingRuleByIdQuery : IRequest<GetFreeShippingRuleByIdResponse>
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Response for getting a specific free shipping rule
    /// </summary>
    public class GetFreeShippingRuleByIdResponse
    {
        public FreeShippingRuleDto? Rule { get; set; }
        public bool Found { get; set; }
    }
}
