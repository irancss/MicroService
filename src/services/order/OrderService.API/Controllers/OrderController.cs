using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OrderService.API.Hubs.Interfaces;
using OrderService.API.Hubs;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHubContext<OrderHub, IOrderHubClient> _hubContext;
        private readonly DaprClient _dapr;
        public OrdersController(
            IHubContext<OrderHub, IOrderHubClient> hubContext,
            DaprClient dapr)
        {
            _hubContext = hubContext;
            _dapr = dapr;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var workflowId = await _dapr.StartWorkflowAsync(...);

            // ارسال نوتیفیکیشن بلادرنگ
            await _hubContext.Clients.Group(dto.UserId)
                .OrderStatusUpdated(new OrderStatusDto
                {
                    OrderId = workflowId,
                    Status = "Processing"
                });

            return Accepted(new { workflowId });
        }
    }
}
