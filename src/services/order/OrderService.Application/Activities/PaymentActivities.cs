using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Events;
using OrderService.Application.Sagas;

namespace OrderService.Application.Activities
{
    public class PaymentActivities : IStateMachineActivity<OrderSagaState>
    {
        private readonly ILogger<PaymentActivities> _logger;

        public PaymentActivities(ILogger<PaymentActivities> logger)
        {
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("process-payment");
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
                _logger.LogInformation("Starting payment processing for order {OrderId}", context.Saga.OrderId);

                // Publish payment processing request
                await context.Publish(new PaymentProcessingRequested
                {
                    OrderId = context.Saga.OrderId,
                    CorrelationId = context.Saga.CorrelationId,
                    CustomerId = context.Saga.CustomerId,
                    Amount = context.Saga.TotalAmount
                });

                _logger.LogInformation("Payment processing request sent for order {OrderId}", context.Saga.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payment for order {OrderId}", context.Saga.OrderId);
                
                await context.Publish(new PaymentFailed
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
            _logger.LogError(context.Exception, "Payment processing activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }

        public Task Faulted<T, TException>(BehaviorExceptionContext<OrderSagaState, T, TException> context, IBehavior<OrderSagaState, T> next) 
            where T : class
            where TException : Exception
        {
            _logger.LogError(context.Exception, "Payment processing activity faulted for order {OrderId}", context.Saga.OrderId);
            return next.Faulted(context);
        }
    }

    public class PaymentProcessingRequested
    {
        public Guid OrderId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
