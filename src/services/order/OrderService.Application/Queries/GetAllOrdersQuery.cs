using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OrderService.Application.Services;
using OrderService.Core.Models;

namespace OrderService.Application.Queries
{
    public class GetAllOrdersQuery : IRequest<IEnumerable<Order>>
    {

    }

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<Order>>
    {
        private readonly IOrderService _orderService;

        public GetAllOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IEnumerable<Order>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _orderService.GetAllOrdersAsync();
        }
    }
}
