using Dapr.Workflow;

namespace OrderApi.Services.Activities
{
    public class ProcessPaymentActivity
    {
        /// <summary>
        /// Process the payment for the order.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <param name="order">The order to process.</param>
        /// <returns>A boolean indicating whether the payment was successful.</returns>
        public async Task<bool> ProcessPaymentAsync(WorkflowContext context, OrderApi.Models.Entities.Order order)
        {
            // Simulate payment processing
            await Task.Delay(1000);
            return true; // Assume payment is always successful for this example
        }
    }
}
