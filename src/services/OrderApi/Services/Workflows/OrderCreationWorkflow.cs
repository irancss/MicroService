using Dapr.Workflow;
using OrderApi.Enums;
using OrderApi.Models.Entities;
using OrderApi.Services.Activities;

namespace OrderApi.Services.Workflows
{
    /// <summary>
    /// Represents the result of an order creation workflow.
    /// </summary>
    public class OrderResult
    {
        /// <summary>
        /// Indicates whether the workflow was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result of the workflow.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderResult"/> class.
        /// </summary>
        /// <param name="success">Whether the workflow succeeded.</param>
        /// <param name="message">Result message.</param>
        public OrderResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    /// <summary>
    /// Workflow for creating an order, including inventory reservation, payment processing, and order status update.
    /// </summary>
    public class OrderCreationWorkflow : Workflow<OrderApi.Models.Entities.Order, OrderResult>
    {
        /// <summary>
        /// Executes the order creation workflow.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <param name="order">The order to process.</param>
        /// <returns>An <see cref="OrderResult"/> indicating the outcome.</returns>
        public override async Task<OrderResult> RunAsync(WorkflowContext context, OrderApi.Models.Entities.Order order)
        {
            try
            {
                // 1. Reserve Inventory
                var inventorySuccess = await context.CallActivityAsync<bool>(
                    nameof(ReserveInventoryActivity),
                    order.Items);

                if (!inventorySuccess)
                    throw new Exception("موجودی کافی نیست");

                // 2. Process Payment
                var paymentSuccess = await context.CallActivityAsync<bool>(
                    nameof(ProcessPaymentActivity),
                    order.TotalPrice);

                if (!paymentSuccess)
                {
                    await CompensateInventory(context, order.Items);
                    return new OrderResult(false, "پرداخت ناموفق");
                }

                // 3. Update Order Status
                order.Status = OrderStatus.Confirmed;
                await context.CallActivityAsync(nameof(UpdateOrderActivity), order);

                return new OrderResult(true, "سفارش ثبت شد");
            }
            catch (Exception ex)
            {
                await CompensateAll(context, order);
                return new OrderResult(false, ex.Message);
            }
        }

        /// <summary>
        /// Compensates all workflow steps in case of failure, including inventory release and payment refund.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <param name="order">The order to compensate.</param>
        private async Task CompensateAll(WorkflowContext context, OrderApi.Models.Entities.Order order)
        {
            await CompensateInventory(context, order.Items);
            await context.CallActivityAsync(nameof(RefundPaymentActivity), order.TotalPrice);
        }

        /// <summary>
        /// Releases reserved inventory for the given order items.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <param name="items">The order items to release inventory for.</param>
        private async Task CompensateInventory(WorkflowContext context, List<OrderItem> items)
        {
            await context.CallActivityAsync(nameof(ReleaseInventoryActivity), items);
        }
    }
}
