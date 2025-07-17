using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Sagas;
using OrderService.Application.Workflows;

namespace OrderService.Application.Activities
{
    public class CompensationActivities : IStateMachineActivity<OrderSagaState>
    {
        private readonly ILogger<CompensationActivities> _logger;

        public CompensationActivities(ILogger<CompensationActivities> logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("compensation");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderSagaState> context, IBehavior<OrderSagaState> next)
        {
            await ExecuteInternal(context);
            await next.Execute(context);
        }

        public async Task Execute<T>(BehaviorContext<OrderSagaState, T> context, IBehavior<OrderSagaState, T> next)
            where T : class
        {
            await ExecuteInternal(context);
            await next.Execute(context);
        }

        private async Task ExecuteInternal(BehaviorContext<OrderSagaState> context)
        {
            try
            {
                _logger.LogInformation("Starting compensation for order {OrderId}", context.Saga.OrderId);

                // Release reserved inventory if it was reserved
                if (context.Saga.InventoryReserved)
                {
                    await context.Publish(new InventoryReleaseRequested
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        Items = context.Saga.Items
                    });
                    _logger.LogInformation("Inventory release requested for order {OrderId}", context.Saga.OrderId);
                }

                // Refund payment if it was processed
                if (context.Saga.PaymentProcessed && !string.IsNullOrEmpty(context.Saga.PaymentTransactionId))
                {
                    await context.Publish(new PaymentRefundRequested
                    {
                        OrderId = context.Saga.OrderId,
                        CorrelationId = context.Saga.CorrelationId,
                        TransactionId = context.Saga.PaymentTransactionId,
                        Amount = context.Saga.TotalAmount
                    });
                    _logger.LogInformation("Payment refund requested for order {OrderId}", context.Saga.OrderId);
                }

                // Publish compensation completed event
                await context.Publish(new CompensationCompleted
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId
                });

                _logger.LogInformation("Compensation completed for order {OrderId}", context.Saga.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete compensation for order {OrderId}", context.Saga.OrderId);
                throw;
            }
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, TException> context, IBehavior<OrderSagaState> next) 
            where TException : Exception
        {
            _logger.LogError(context.Exception, "Compensation activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<OrderSagaState, T, TException> context, IBehavior<OrderSagaState, T> next) 
            where T : class
            where TException : Exception
        {
            _logger.LogError(context.Exception, "Compensation activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }
    }

    public class InventoryReleaseRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public List<Core.Models.OrderItem> Items { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }

    public class PaymentRefundRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
