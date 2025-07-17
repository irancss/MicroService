using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Queries;
using ShippingService.Domain.Services;
using AutoMapper;

namespace ShippingService.Application.Handlers.Queries.FreeShippingRules
{
    /// <summary>
    /// Handler for getting active free shipping rules
    /// </summary>
    public class GetActiveRulesQueryHandler : IRequestHandler<GetActiveRulesQuery, IEnumerable<DTOs.FreeShippingRuleDto>>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;
        private readonly IMapper _mapper;

        public GetActiveRulesQueryHandler(IFreeShippingRuleService freeShippingRuleService, IMapper mapper)
        {
            _freeShippingRuleService = freeShippingRuleService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DTOs.FreeShippingRuleDto>> Handle(GetActiveRulesQuery request, CancellationToken cancellationToken)
        {
            var activeRules = await _freeShippingRuleService.GetActiveRulesAsync();
            return _mapper.Map<IEnumerable<DTOs.FreeShippingRuleDto>>(activeRules);
        }
    }
}
