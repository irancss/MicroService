using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using OrderService.Core.Models;

namespace OrderService.Application.Commands
{
    public class UpdateOrderCommand : IRequest<OrderVM>
    {
        public Guid Id { get; set; }
        // Add properties to update as needed, e.g.:
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderVM>
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public UpdateOrderCommandHandler(IMapper mapper, IOrderService orderService)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<OrderVM> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderService.GetOrderByIdAsync(request.Id);
            if (existingOrder == null)
            {
                // Handle not found, throw exception or return null as per your policy
                return null;
            }

            // Map updated fields from request to existingOrder
            _mapper.Map(request, existingOrder);

            var updated = await _orderService.UpdateOrderAsync(existingOrder);
            if (!updated)
            {
                // Handle update failure as needed
                return null;
            }

            var orderVm = _mapper.Map<OrderVM>(existingOrder);
            return orderVm;
        }
    }
}
