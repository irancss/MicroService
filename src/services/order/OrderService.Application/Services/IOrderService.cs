using OrderService.Core.Enums;
using OrderService.Core.Models;

namespace OrderService.Application.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">Order entity to create.</param>
        /// <returns>True if the creation was successful, otherwise false.</returns>
        Task<bool> CreateOrderAsync(Order order);
        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order">Order entity to update.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        Task<bool> UpdateOrderAsync(Order order);
        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteOrderAsync(Guid id);
        /// <summary>
        /// Changes the status of an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="status">New order status.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        Task<bool> ChangeOrderStatusAsync(Guid id, OrderStatus status);


        /// <summary>
        /// Gets an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>The order if found, otherwise null.</returns>
        Task<Order?> GetOrderByIdAsync(Guid id);

        /// <summary>
        /// Gets all orders for a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>List of orders for the user.</returns>
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int pageNumber , int pageSize ,string sortBy, bool sortDescending );

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <returns>List of all orders.</returns>
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        /// <summary>
        /// Gets orders by custom filter.
        /// </summary>
        /// <param name="filter">A function to filter orders.</param>
        /// <returns>List of orders matching the filter.</returns>
        Task<IEnumerable<Order>> GetOrdersByFilterAsync(Func<Order, bool> filter);

        /// <summary>
        /// Gets orders within a date range.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns>List of orders within the date range.</returns>
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets orders for a specific user with pagination.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortBy">Sort field.</param>
        /// <param name="sortAscending">Sort order.</param>
        /// <returns>List of orders for the user.</returns>
        Task<IEnumerable<Order>> GetOrdersByUserAsync(Guid userId, int pageNumber, int pageSize, string sortBy, bool sortAscending);

        /// <summary>
        /// Gets total count of orders for a specific user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>Total count of user orders.</returns>
        Task<int> GetUserOrdersCountAsync(Guid userId);

        /// <summary>
        /// Gets orders with filtering and pagination for dashboard.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="status">Optional status filter.</param>
        /// <param name="fromDate">Optional from date filter.</param>
        /// <param name="toDate">Optional to date filter.</param>
        /// <param name="sortBy">Sort field.</param>
        /// <param name="sortAscending">Sort order.</param>
        /// <returns>List of filtered orders.</returns>
        Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize, OrderStatus? status, DateTime? fromDate, DateTime? toDate, string sortBy, bool sortAscending);

        /// <summary>
        /// Gets total count of orders with filters.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="fromDate">Optional from date filter.</param>
        /// <param name="toDate">Optional to date filter.</param>
        /// <returns>Total count of filtered orders.</returns>
        Task<int> GetOrdersCountAsync(OrderStatus? status, DateTime? fromDate, DateTime? toDate);
    }
}
