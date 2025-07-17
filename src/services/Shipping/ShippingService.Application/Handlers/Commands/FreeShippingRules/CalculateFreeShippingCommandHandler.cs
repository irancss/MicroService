using MediatR;
using ShippingService.Application.Features.FreeShippingRules.Commands;
using ShippingService.Domain.Services;

namespace ShippingService.Application.Handlers.Commands.FreeShippingRules
{
    /// <summary>
    /// Handler for calculating free shipping eligibility
    /// </summary>
    public class CalculateFreeShippingCommandHandler : IRequestHandler<CalculateFreeShippingCommand, FreeShippingCalculationResult>
    {
        private readonly IFreeShippingRuleService _freeShippingRuleService;

        public CalculateFreeShippingCommandHandler(IFreeShippingRuleService freeShippingRuleService)
        {
            _freeShippingRuleService = freeShippingRuleService;
        }

        public async Task<FreeShippingCalculationResult> Handle(CalculateFreeShippingCommand request, CancellationToken cancellationToken)
        {
            // Create context for rule evaluation
            var context = new Dictionary<string, object>
            {
                { "UserId", request.UserId },
                { "OrderAmount", request.OrderAmount },
                { "ItemCount", request.ItemCount },
                { "TotalWeight", request.TotalWeight },
                { "ProductCategory", request.ProductCategory },
                { "ShippingMethodId", request.ShippingMethodId },
                { "DestinationPostalCode", request.DestinationPostalCode },
                { "DestinationCity", request.DestinationCity },
                { "DayOfWeek", DateTime.Now.DayOfWeek.ToString() }
            };

            // Calculate original shipping cost (this would typically come from another service)
            decimal originalCost = 50000; // Default shipping cost

            // Apply free shipping rules
            var result = await _freeShippingRuleService.CalculateFreeShippingAsync(context, originalCost);

            return new FreeShippingCalculationResult
            {
                IsEligible = result.IsEligible,
                OriginalCost = originalCost,
                DiscountAmount = result.DiscountAmount,
                FinalCost = result.FinalCost,
                AppliedRuleName = result.AppliedRuleName,
                RuleDescription = result.RuleDescription
            };
        }
    }
}
