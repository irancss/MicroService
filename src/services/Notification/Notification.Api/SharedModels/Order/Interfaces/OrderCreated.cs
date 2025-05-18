namespace Notification.Api.SharedModels.Order.Interfaces
{
    public interface IOrderCreated
    {
        int Id { get; set; }
        string ProductName { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }
    }
}
