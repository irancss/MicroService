using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderService.Core.Models;

namespace OrderService.Application.Queries
{
    public class GetCustomOrdersQuery : IRequest<IEnumerable<Order>> { /* filter properties */ }
}
