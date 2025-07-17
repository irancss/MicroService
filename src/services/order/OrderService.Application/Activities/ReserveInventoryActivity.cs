using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Events;
using OrderService.Application.Sagas;

namespace OrderService.Application.Activities
{
    public class ReserveInventoryActivity : IStateMachineActivity<OrderSagaState>
    {
        private readonly ILogger<ReserveInventoryActivity> _logger;

        public ReserveInventoryActivity(ILogger<ReserveInventoryActivity> logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("reserve-inventory");
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
                _logger.LogInformation("Starting inventory reservation for order {OrderId}", context.Saga.OrderId);

                // Publish inventory reservation request
                await context.Publish(new InventoryReservationRequested
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    Items = context.Saga.Items
                });

                _logger.LogInformation("Inventory reservation request sent for order {OrderId}", context.Saga.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reserve inventory for order {OrderId}", context.Saga.OrderId);
                
                await context.Publish(new InventoryReservationFailed
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    Reason = ex.Message
                });
            }
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderSagaState, TException> context, IBehavior<OrderSagaState> next) 
            where TException : Exception
        {
            _logger.LogError(context.Exception, "Inventory reservation activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<OrderSagaState, T, TException> context, IBehavior<OrderSagaState, T> next) 
            where T : class
            where TException : Exception
        {
            _logger.LogError(context.Exception, "Inventory reservation activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }
    }

    public class InventoryReservationRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public List<Core.Models.OrderItem> Items { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
