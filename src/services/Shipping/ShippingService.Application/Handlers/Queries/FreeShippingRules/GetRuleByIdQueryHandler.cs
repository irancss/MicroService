using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Queries;
using ShippingService.Domain.Services;
using AutoMapper;

namespace ShippingService.Application.Handlers.Queries.FreeShippingRules
{
    /// <summary>
    /// Handler for getting a specific free shipping rule by ID
    /// </summary>
    public class GetRuleByIdQueryHandler : IRequestHandler<GetRuleByIdQuery, DTOs.FreeShippingRuleDto?>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;
        private readonly IMapper _mapper;

        public GetRuleByIdQueryHandler(IFreeShippingRuleService freeShippingRuleService, IMapper mapper)
        {
            _freeShippingRuleService = freeShippingRuleService;
            _mapper = mapper;
        }

        public async Task<DTOs.FreeShippingRuleDto?> Handle(GetRuleByIdQuery request, CancellationToken cancellationToken)
        {
            var rule = await _freeShippingRuleService.GetRuleByIdAsync(request.RuleId);
            return rule != null ? _mapper.Map<DTOs.FreeShippingRuleDto>(rule) : null;
        }
    }
}
