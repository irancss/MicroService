using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using OrderService.Core.Enums;
using OrderService.Core.Models;
using OrderService.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Commands
{
    public class CreateOrderCommand : IRequest<OrderVM> {
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderVM>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrderService orderService, 
            IMapper mapper,
            IPublishEndpoint publishEndpoint,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<OrderVM> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting order creation process for customer {CustomerId}", request.CustomerId);

                // Create initial order entity with pending status
                var order = _mapper.Map<Order>(request);
                order.Id = Guid.NewGuid();
                order.Status = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;
                order.TotalAmount = order.TotalPrice - order.TotalDiscount;
                order.CreatedAt = DateTime.UtcNow;
                order.LastUpdatedAt = DateTime.UtcNow;
                order.LastUpdatedBy = "system";

                // Assign new Ids to OrderItems and set OrderId
                foreach (var item in order.Items)
                {
                    item.Id = Guid.NewGuid();
                    item.OrderId = order.Id;
                    item.UnitPrice = item.UnitPrice > 0 ? item.UnitPrice : 0;
                    item.Quantity = item.Quantity > 0 ? item.Quantity : 1;
                    item.Discount = item.Discount > 0 ? item.Discount : 0;
                }

                // Save initial order to database
                var result = await _orderService.CreateOrderAsync(order);
                if (!result)
                {
                    _logger.LogError("Failed to create initial order for customer {CustomerId}", request.CustomerId);
                    throw new Exception("Failed to create order.");
                }

                _logger.LogInformation("Initial order {OrderId} created, starting saga workflow", order.Id);

                // Start the Order Creation Saga
                await _publishEndpoint.Publish(new OrderCreationStarted
                {
                    OrderId = order.Id,
                    CustomerId = order.CustomerId,
                    TotalAmount = order.TotalAmount,
                    ShippingAddress = order.ShippingAddress,
                    BillingAddress = order.BillingAddress,
                    Items = order.Items
                }, cancellationToken);

                _logger.LogInformation("Order creation saga started for order {OrderId}", order.Id);

                // Return the order view model
                var orderVm = _mapper.Map<OrderVM>(order);
                return orderVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}
