using MediatR;
using OrderService.Core.Models;
using OrderService.Application.Services;
using OrderService.Application.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderVM?>
    {
        public Guid Id { get; set; }
    }

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderVM?>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IOrderService orderService, 
            IMapper mapper,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderVM?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving order {OrderId}", request.Id);

                var order = await _orderService.GetOrderByIdAsync(request.Id);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", request.Id);
                    return null;
                }

                var orderVm = _mapper.Map<OrderVM>(order);
                
                _logger.LogInformation("Successfully retrieved order {OrderId}", request.Id);
                return orderVm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", request.Id);
                throw;
            }
        }
    }
}
