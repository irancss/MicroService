
using AutoMapper;
using MediatR;
using OrderService.Application.Services;
using OrderService.Core.Enums;
using OrderService.Core.Models;

namespace OrderService.Application.Queries
{
    public class GetOrdersByUserQuery : IRequest<IEnumerable<Order>>
    {
        public Guid UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }

    public class GetOrdersByUserQueryHandler : IRequestHandler<GetOrdersByUserQuery, IEnumerable<Order>>
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public GetOrdersByUserQueryHandler(IMapper mapper , IOrderService orderService)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Order>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
        {
           return await _orderService.GetOrdersByUserIdAsync(request.UserId, request.PageNumber, request.PageSize, request.SortBy, request.SortDescending);
        }
    }

}
