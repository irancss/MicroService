using AutoMapper;
using OrderService.Core.Enums;
using OrderService.Core.Models;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<bool> ChangeOrderStatusAsync(Guid id, OrderStatus status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return false;

            order.Status = status;
            order.LastUpdatedAt = DateTime.UtcNow;
            return await _orderRepository.ChangeOrderStatusAsync(id, status);
        }

        public async Task<bool> CreateOrderAsync(Order order)
        {
            if (order == null)
                return false;

            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            order.LastUpdatedAt = DateTime.UtcNow;
            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    item.Id = Guid.NewGuid();
                    item.OrderId = order.Id;
                }
            }
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return false;

            return await _orderRepository.DeleteOrderAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize, string sortBy, bool sortDescending)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId, pageNumber, pageSize, sortBy, sortDescending, string.Empty);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Example: fetch all orders, no paging/sorting/filtering for now
            return await _orderRepository.GetAllOrdersAsync(1, int.MaxValue, "CreatedAt", false);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Order>> GetOrdersByFilterAsync(Func<Order, bool> filter)
        {
            // No direct support in repository, so fetch all and filter in memory
            var allOrders = await _orderRepository.GetAllOrdersAsync(1, int.MaxValue, "CreatedAt", false);
            return allOrders.Where(filter);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            // Example: fetch all orders for user, no paging/sorting/filtering for now
            return await _orderRepository.GetOrdersByUserIdAsync(userId, 1, int.MaxValue, "CreatedAt", false, string.Empty);
        }

        public Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            if (order == null)
                return false;

            var existingOrder = await _orderRepository.GetOrderByIdAsync(order.Id);
            if (existingOrder == null)
                return false;

            order.LastUpdatedAt = DateTime.UtcNow;
            return await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(Guid userId, int pageNumber, int pageSize, string sortBy, bool sortAscending)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId, pageNumber, pageSize, sortBy, !sortAscending, string.Empty);
        }

        public async Task<int> GetUserOrdersCountAsync(Guid userId)
        {
            // Assuming repository has a count method or we implement it
            var allUserOrders = await _orderRepository.GetOrdersByUserIdAsync(userId, 1, int.MaxValue, "CreatedAt", false, string.Empty);
            return allUserOrders.Count();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize, OrderStatus? status, DateTime? fromDate, DateTime? toDate, string sortBy, bool sortAscending)
        {
            // This would need to be implemented in the repository layer as well
            // For now, implementing a basic version
            var allOrders = await _orderRepository.GetAllOrdersAsync(pageNumber, pageSize, sortBy, !sortAscending);
            
            var filteredOrders = allOrders.AsEnumerable();
            
            if (status.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.Status == status.Value);
            }
            
            if (fromDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.CreatedAt >= fromDate.Value);
            }
            
            if (toDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.CreatedAt <= toDate.Value);
            }
            
            return filteredOrders;
        }

        public async Task<int> GetOrdersCountAsync(OrderStatus? status, DateTime? fromDate, DateTime? toDate)
        {
            // Get all orders and apply filters to count
            var allOrders = await _orderRepository.GetAllOrdersAsync(1, int.MaxValue, "CreatedAt", false);
            
            var filteredOrders = allOrders.AsEnumerable();
            
            if (status.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.Status == status.Value);
            }
            
            if (fromDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.CreatedAt >= fromDate.Value);
            }
            
            if (toDate.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => o.CreatedAt <= toDate.Value);
            }
            
            return filteredOrders.Count();
        }
    }
}
