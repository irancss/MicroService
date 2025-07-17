using ShippingService.Domain.Entities;

namespace ShippingService.Application.Services
{
    /// <summary>
    /// Interface for free shipping rule service
    /// </summary>
    public interface IFreeShippingRuleService
    {
        /// <summary>
        /// Creates a new free shipping rule
        /// </summary>
        /// <param name="rule">The rule to create</param>
        /// <returns>The created rule</returns>
        Task<FreeShippingRule> CreateRuleAsync(FreeShippingRule rule);

        /// <summary>
        /// Updates an existing free shipping rule
        /// </summary>
        /// <param name="rule">The rule to update</param>
        /// <returns>The updated rule</returns>
        Task<FreeShippingRule> UpdateRuleAsync(FreeShippingRule rule);

        /// <summary>
        /// Deletes a free shipping rule
        /// </summary>
        /// <param name="id">The ID of the rule to delete</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteRuleAsync(Guid id);

        /// <summary>
        /// Gets a free shipping rule by ID
        /// </summary>
        /// <param name="id">The ID of the rule</param>
        /// <returns>The rule if found</returns>
        Task<FreeShippingRule?> GetRuleByIdAsync(Guid id);

        /// <summary>
        /// Gets all free shipping rules
        /// </summary>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>List of rules</returns>
        Task<List<FreeShippingRule>> GetAllRulesAsync(bool? isActive = null);

        /// <summary>
        /// Evaluates if free shipping applies to a shipment
        /// </summary>
        /// <param name="shipment">The shipment to evaluate</param>
        /// <returns>Discount amount if applicable</returns>
        Task<decimal> EvaluateFreeShippingAsync(Shipment shipment);

        /// <summary>
        /// Applies free shipping rule usage
        /// </summary>
        /// <param name="ruleId">The rule ID</param>
        /// <param name="shipmentId">The shipment ID</param>
        /// <returns>True if applied successfully</returns>
        Task<bool> ApplyRuleUsageAsync(Guid ruleId, Guid shipmentId);
    }
}
