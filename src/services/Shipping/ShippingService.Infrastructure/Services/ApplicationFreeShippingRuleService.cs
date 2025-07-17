using ShippingService.Application.Services;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Services;

namespace ShippingService.Infrastructure.Services
{
    /// <summary>
    /// Application layer implementation of IFreeShippingRuleService that wraps the domain service
    /// </summary>
    public class ApplicationFreeShippingRuleService : ShippingService.Application.Services.IFreeShippingRuleService
    {
        private readonly ShippingService.Domain.Services.IFreeShippingRuleService _domainService;

        public ApplicationFreeShippingRuleService(ShippingService.Domain.Services.IFreeShippingRuleService domainService)
        {
            _domainService = domainService;
        }

        public async Task<FreeShippingRule> CreateRuleAsync(FreeShippingRule rule)
        {
            return await _domainService.CreateRuleAsync(rule);
        }

        public async Task<FreeShippingRule> UpdateRuleAsync(FreeShippingRule rule)
        {
            var success = await _domainService.UpdateRuleAsync(rule);
            if (!success)
                throw new InvalidOperationException("Failed to update rule");
            return rule;
        }

        public async Task<bool> DeleteRuleAsync(Guid id)
        {
            return await _domainService.DeleteRuleAsync(id);
        }

        public async Task<FreeShippingRule?> GetRuleByIdAsync(Guid id)
        {
            return await _domainService.GetRuleByIdAsync(id);
        }

        public async Task<List<FreeShippingRule>> GetAllRulesAsync(bool? isActive = null)
        {
            if (isActive.HasValue && isActive.Value)
            {
                var activeRules = await _domainService.GetActiveRulesAsync();
                return activeRules.ToList();
            }
            
            // For non-active or all rules, we would need a different domain method
            // For now, just return active rules
            var rules = await _domainService.GetActiveRulesAsync();
            return rules.ToList();
        }

        public async Task<decimal> EvaluateFreeShippingAsync(Shipment shipment)
        {
            var shipmentData = new ShipmentData
            {
                UserId = shipment.CustomerId,
                OrderAmount = shipment.TotalCost,
                ItemCount = 1, // Default - would need actual item count from shipment
                TotalWeight = shipment.Weight,
                ShippingMethodId = shipment.ShippingMethodId,
                DestinationCity = shipment.DestinationCity,
                ShippingDate = DateTime.UtcNow,
                UserLevel = "Standard" // Default - would need to get from user profile
            };

            var (rule, discount) = await _domainService.CalculateFreeShippingAsync(shipmentData, false);
            return discount;
        }

        public Task<bool> ApplyRuleUsageAsync(Guid ruleId, Guid shipmentId)
        {
            // This would typically track usage in the database
            // For now, just return success
            return Task.FromResult(true);
        }
    }
}
