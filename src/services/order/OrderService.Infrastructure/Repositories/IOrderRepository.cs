using OrderService.Core.Enums;
using OrderService.Core.Models;

namespace OrderService.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize, string sortBy, bool sortDescending, string filter);
        Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string sortBy, bool sortDescending);
        Task<IEnumerable<Order>> GetCustomOrdersAsync(string filter);
        Task<bool> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<bool> ChangeOrderStatusAsync(Guid id, OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
