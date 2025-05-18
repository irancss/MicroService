namespace OrderService.API.Hubs.Interfaces
{
    public interface IOrderHubClient
    {
        Task OrderStatusUpdated(OrderStatusDto status);
        Task OrderCancelled(string reason);
    }
}
