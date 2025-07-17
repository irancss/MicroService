using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Core.Enums;

namespace OrderService.API.Controllers
{
    /// <summary>
    /// Controller for managing orders using CQRS pattern with MediatR.
    /// Supports order creation via Saga workflow, status updates, cancellations, and comprehensive querying.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ApiControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order and starts the Order Creation Saga workflow.
        /// </summary>
        /// <param name="command">The create order command.</param>
        /// <returns>The created order.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderVM), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                _logger.LogInformation("Creating new order for customer {CustomerId}", command.CustomerId);
                var result = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", command.CustomerId);
                return BadRequest(new { message = "Failed to create order", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancels an order by publishing a cancellation request to the saga.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="request">Cancellation request details.</param>
        /// <returns>Confirmation of cancellation request.</returns>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
        {
            try
            {
                var command = new CancelOrderCommand 
                { 
                    OrderId = id, 
                    Reason = request.Reason ?? "Cancelled by user",
                    CancelledBy = request.CancelledBy ?? "User"
                };
                
                var result = await Mediator.Send(command);
                if (!result)
                    return NotFound(new { message = "Order not found or cannot be cancelled" });

                return Ok(new { message = "Cancellation request submitted", orderId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", id);
                return BadRequest(new { message = "Failed to cancel order", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific order by ID.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <returns>The order if found.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderVM), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var query = new GetOrderByIdQuery { Id = id };
                var result = await Mediator.Send(query);
                if (result == null)
                    return NotFound(new { message = "Order not found", orderId = id });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", id);
                return BadRequest(new { message = "Failed to retrieve order", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets orders for a specific user with pagination.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="pageNumber">Page number (default: 1).</param>
        /// <param name="pageSize">Page size (default: 10).</param>
        /// <param name="sortBy">Sort field (default: CreatedAt).</param>
        /// <param name="sortAscending">Sort order (default: false).</param>
        /// <returns>Paginated list of user orders.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(PaginatedResult<OrderVM>), 200)]
        public async Task<IActionResult> GetUserOrders(
            Guid userId, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool sortAscending = false)
        {
            try
            {
                var query = new GetUserOrdersQuery
                {
                    UserId = userId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortAscending = sortAscending
                };

                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                return BadRequest(new { message = "Failed to retrieve user orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Gets orders for dashboard with filtering and pagination (admin only).
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1).</param>
        /// <param name="pageSize">Page size (default: 20).</param>
        /// <param name="status">Optional status filter.</param>
        /// <param name="fromDate">Optional from date filter.</param>
        /// <param name="toDate">Optional to date filter.</param>
        /// <param name="sortBy">Sort field (default: CreatedAt).</param>
        /// <param name="sortAscending">Sort order (default: false).</param>
        /// <returns>Paginated list of filtered orders.</returns>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(PaginatedResult<OrderVM>), 200)]
        public async Task<IActionResult> GetDashboardOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] OrderStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool sortAscending = false)
        {
            try
            {
                var query = new GetDashboardOrdersQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    SortBy = sortBy,
                    SortAscending = sortAscending
                };

                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard orders");
                return BadRequest(new { message = "Failed to retrieve dashboard orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Updates order status (internal use by saga or admin).
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="request">Status update request.</param>
        /// <returns>Confirmation of status update.</returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var command = new UpdateOrderStatusCommand
                {
                    OrderId = id,
                    Status = request.Status,
                    UpdatedBy = request.UpdatedBy ?? "User",
                    Notes = request.Notes
                };

                var result = await Mediator.Send(command);
                if (!result)
                    return NotFound(new { message = "Order not found", orderId = id });

                return Ok(new { message = "Order status updated successfully", orderId = id, status = request.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", id);
                return BadRequest(new { message = "Failed to update order status", error = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
        public string? CancelledBy { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Notes { get; set; }
    }
}
