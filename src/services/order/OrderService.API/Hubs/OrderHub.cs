using Microsoft.AspNetCore.SignalR;
using OrderService.API.Hubs.Interfaces;

namespace OrderService.API.Hubs
{
    public class OrderHub : Hub<IOrderHubClient>
    {
        public async Task SubscribeToOrder(Guid orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
        }
    }
}
