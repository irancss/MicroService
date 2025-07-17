using ShippingService.Domain.Entities;
using ShippingService.Domain.Enums;

namespace ShippingService.Application.Services
{
    /// <summary>
    /// Interface for premium subscription service
    /// </summary>
    public interface IPremiumSubscriptionService
    {
        /// <summary>
        /// Creates a new premium subscription
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="name">The subscription name</param>
        /// <param name="subscriptionType">The subscription type</param>
        /// <param name="freeRequestsPerMonth">Number of free requests per month</param>
        /// <returns>The created subscription</returns>
        Task<PremiumSubscription> CreateSubscriptionAsync(string userId, string name, SubscriptionType subscriptionType, int freeRequestsPerMonth);

        /// <summary>
        /// Gets active subscription for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>The active subscription if found</returns>
        Task<PremiumSubscription?> GetActiveSubscriptionAsync(string userId);

        /// <summary>
        /// Checks if user can use free shipping request
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>True if can use free request</returns>
        Task<bool> CanUseFreeRequestAsync(string userId);

        /// <summary>
        /// Uses a free shipping request
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="shipmentId">The shipment ID</param>
        /// <param name="description">Usage description</param>
        /// <returns>True if used successfully</returns>
        Task<bool> UseFreeRequestAsync(string userId, Guid shipmentId, string description);

        /// <summary>
        /// Extends a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="additionalMonths">Number of additional months</param>
        /// <returns>The updated subscription</returns>
        Task<PremiumSubscription> ExtendSubscriptionAsync(Guid subscriptionId, int additionalMonths);

        /// <summary>
        /// Cancels a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <returns>The cancelled subscription</returns>
        Task<PremiumSubscription> CancelSubscriptionAsync(Guid subscriptionId);

        /// <summary>
        /// Gets subscription usage history
        /// </summary>
        /// <param name="subscriptionId">The subscription ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of usage logs</returns>
        Task<List<SubscriptionUsageLog>> GetUsageHistoryAsync(Guid subscriptionId, int pageNumber = 1, int pageSize = 10);
    }
}
