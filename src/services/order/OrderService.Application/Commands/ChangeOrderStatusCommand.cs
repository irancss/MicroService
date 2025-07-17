using AutoMapper;
using MediatR;
using OrderService.Application.Services;
using OrderService.Core.Enums;
using OrderService.Core.Models;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Commands
{
    public class ChangeOrderStatusCommand : IRequest<Order>
    {
        public Guid Id { get; set; } 
        public OrderStatus Status { get; set; }
    }

    // Add alias for saga usage
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string UpdatedBy { get; set; } = "System";
        public string? Notes { get; set; }
    }

    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, Order>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public ChangeOrderStatusCommandHandler(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<Order> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Get the order by ID
            var order = await _orderService.GetOrderByIdAsync(request.Id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with Id {request.Id} not found.");
            }

            // Change the order status
            var statusChanged = await _orderService.ChangeOrderStatusAsync(request.Id, request.Status);
            if (!statusChanged)
            {
                throw new InvalidOperationException("Failed to change order status.");
            }

            // Retrieve the updated order
            var updatedOrder = await _orderService.GetOrderByIdAsync(request.Id);
            if (updatedOrder == null)
            {
                throw new InvalidOperationException("Order not found after status update.");
            }

            return updatedOrder;
        }
    }

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

        public UpdateOrderStatusCommandHandler(
            IOrderService orderService,
            ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating order status for order {OrderId} to {Status}", 
                    request.OrderId, request.Status);

                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found for status update", request.OrderId);
                    return false;
                }

                order.Status = request.Status;
                order.LastUpdatedAt = DateTime.UtcNow;
                order.LastUpdatedBy = request.UpdatedBy;

                var result = await _orderService.UpdateOrderAsync(order);
                
                if (result)
                {
                    _logger.LogInformation("Successfully updated order {OrderId} status to {Status}", 
                        request.OrderId, request.Status);
                }
                else
                {
                    _logger.LogError("Failed to update order {OrderId} status to {Status}", 
                        request.OrderId, request.Status);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status to {Status}", 
                    request.OrderId, request.Status);
                return false;
            }
        }
    }

}
