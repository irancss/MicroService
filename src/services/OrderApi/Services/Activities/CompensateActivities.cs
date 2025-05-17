using Dapr.Workflow;

namespace OrderApi.Services.Activities
{
    public class CompensateActivities
    {
        /// <summary>
        /// Compensate the order by releasing inventory and refunding payment.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <param name="order">The order to compensate.</param>
        /// <returns>A boolean indicating whether the compensation was successful.</returns>
        public async Task<bool> CompensateOrderAsync(WorkflowContext context, OrderApi.Models.Entities.Order order)
        {
            // 1. Release Inventory
            var inventorySuccess = await context.CallActivityAsync<bool>(
                nameof(ReleaseInventoryActivity),
                order.Items);
            // 2. Refund Payment
            var paymentSuccess = await context.CallActivityAsync<bool>(
                nameof(RefundPaymentActivity),
                order);
            return inventorySuccess && paymentSuccess;
        }
      
    }
}
