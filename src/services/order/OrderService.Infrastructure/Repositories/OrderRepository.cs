using Microsoft.EntityFrameworkCore;
using OrderService.Core.Enums;
using OrderService.Core.Models;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for orders.</param>
        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Changes the status of an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="status">New order status.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> ChangeOrderStatusAsync(Guid id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;
            order.Status = status;
            order.LastUpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">Order entity to create.</param>
        /// <returns>True if the creation was successful, otherwise false.</returns>
        public async Task<bool> CreateOrderAsync(Order order)
        {
            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;
            order.LastUpdatedAt = DateTime.UtcNow;
            await _context.Orders.AddAsync(order);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Deletes an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;
            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Gets all orders with pagination and sorting.
        /// </summary>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="sortDescending">Sort descending if true.</param>
        /// <returns>Paged and sorted list of orders.</returns>
        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int pageNumber, int pageSize, string sortBy, bool sortDescending)
        {
            var query = _context.Orders.AsQueryable();
            query = ApplySorting(query, sortBy, sortDescending);
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Gets orders matching a custom filter (searches in ShippingAddress, BillingAddress, and Status).
        /// </summary>
        /// <param name="filter">Filter string.</param>
        /// <returns>List of orders matching the filter.</returns>
        public async Task<IEnumerable<Order>> GetCustomOrdersAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return await _context.Orders.ToListAsync();

            filter = filter.ToLower();
            return await _context.Orders
                .Where(o =>
                    o.ShippingAddress.ToLower().Contains(filter) ||
                    o.BillingAddress.ToLower().Contains(filter) ||
                    o.Status.ToString().ToLower().Contains(filter))
                .ToListAsync();
        }

        /// <summary>
        /// Gets an order by its ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>The order if found, otherwise null.</returns>
        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Gets orders within a specific date range.
        /// </summary>
        /// <param name="startDate">Start date (inclusive).</param>
        /// <param name="endDate">End date (inclusive).</param>
        /// <returns>List of orders in the date range.</returns>
        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets orders for a specific user with pagination, sorting, and filtering.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="pageNumber">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="sortDescending">Sort descending if true.</param>
        /// <param name="filter">Filter string.</param>
        /// <returns>Paged, sorted, and filtered list of user's orders.</returns>
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int pageNumber, int pageSize, string sortBy, bool sortDescending, string filter)
        {
            var query = _context.Orders.Where(o => o.CustomerId == userId);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var lowerFilter = filter.ToLower();
                query = query.Where(o =>
                    o.ShippingAddress.ToLower().Contains(lowerFilter) ||
                    o.BillingAddress.ToLower().Contains(lowerFilter) ||
                    o.Status.ToString().ToLower().Contains(lowerFilter));
            }

            query = ApplySorting(query, sortBy, sortDescending);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="order">Order entity with updated values.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder == null) return false;

            // Update fields
            existingOrder.TotalPrice = order.TotalPrice;
            existingOrder.TotalDiscount = order.TotalDiscount;
            existingOrder.Status = order.Status;
            existingOrder.ShippingAddress = order.ShippingAddress;
            existingOrder.BillingAddress = order.BillingAddress;
            existingOrder.PaymentStatus = order.PaymentStatus;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.LastUpdatedAt = DateTime.UtcNow;
            existingOrder.LastUpdatedBy = order.LastUpdatedBy;

            _context.Orders.Update(existingOrder);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Applies sorting to the query based on the property name and direction.
        /// </summary>
        /// <param name="query">Order query.</param>
        /// <param name="sortBy">Property name to sort by.</param>
        /// <param name="sortDescending">Sort descending if true.</param>
        /// <returns>Sorted query.</returns>
        private IQueryable<Order> ApplySorting(IQueryable<Order> query, string sortBy, bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query.OrderByDescending(o => o.CreatedAt);

            // Only allow sorting by known properties
            switch (sortBy.ToLower())
            {
                case "createdat":
                    query = sortDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt);
                    break;
                case "totalprice":
                    query = sortDescending ? query.OrderByDescending(o => o.TotalPrice) : query.OrderBy(o => o.TotalPrice);
                    break;
                case "status":
                    query = sortDescending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status);
                    break;
                default:
                    query = query.OrderByDescending(o => o.CreatedAt);
                    break;
            }
            return query;
        }
    }
    }

