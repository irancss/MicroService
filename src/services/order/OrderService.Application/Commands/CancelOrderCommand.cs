using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Services;
using OrderService.Application.Workflows;
using MassTransit;

namespace OrderService.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = "Cancelled by user";
        public string CancelledBy { get; set; } = "User";
    }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderService _orderService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(
            IOrderService orderService,
            IPublishEndpoint publishEndpoint,
            ILogger<CancelOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing cancellation request for order {OrderId}", request.OrderId);

                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found for cancellation", request.OrderId);
                    return false;
                }

                // Check if order can be cancelled
                if (order.Status == Core.Enums.OrderStatus.Delivered || 
                    order.Status == Core.Enums.OrderStatus.Cancelled)
                {
                    _logger.LogWarning("Cannot cancel order {OrderId} with status {Status}", 
                        request.OrderId, order.Status);
                    return false;
                }

                // Publish cancellation request to saga
                await _publishEndpoint.Publish(new OrderCancellationRequested
                {
                    OrderId = request.OrderId,
                    CorrelationId = request.OrderId, // Using OrderId as correlation for simplicity
                    Reason = request.Reason
                }, cancellationToken);

                _logger.LogInformation("Cancellation request published for order {OrderId}", request.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing cancellation for order {OrderId}", request.OrderId);
                return false;
            }
        }
    }
}
