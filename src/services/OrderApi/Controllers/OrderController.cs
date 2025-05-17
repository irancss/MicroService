using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Dapr.Client;

namespace OrderApi.Controllers
{
    /// <summary>
    /// Controller for managing orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        /// <summary>
        /// In-memory order storage for demonstration purposes.
        /// </summary>
        private static readonly List<Order> Orders = new List<Order>();

        /// <summary>
        /// Next order ID for in-memory storage.
        /// </summary>
        private static int _nextId = 1;

    
     
        private readonly DaprClient _dapr;
        private readonly OrderDbContext _db;

        public OrdersController(DaprClient dapr, OrderDbContext db)
        {
            _dapr = dapr;
            _db = db;
        }

        /// <summary>
        /// Adds a new order.
        /// </summary>
        /// <param name="orderDto">Order data transfer object.</param>
        /// <returns>The created order or a bad request result.</returns>
        [HttpPost]
        [Route("add")]
        public IActionResult AddOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || string.IsNullOrWhiteSpace(orderDto.ProductName))
                return BadRequest("Invalid order data.");

            var order = new Order
            {
                Id = _nextId++,
                ProductName = orderDto.ProductName,
                Quantity = orderDto.Quantity,
                UserId = orderDto.UserId,
                Status = "Active"
            };
            Orders.Add(order);
            return Ok(order);

            var workflowId = await _dapr.StartWorkflowAsync(
                "dapr",
                "OrderCreationWorkflow",
                order,
                id: Guid.NewGuid().ToString());

            return Accepted(new { workflowId });
        }

        /// <summary>
        /// Edits an existing order.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="orderDto">Order data transfer object.</param>
        /// <returns>The updated order or a not found result.</returns>
        [HttpPut]
        [Route("edit/{id}")]
        public IActionResult EditOrder(int id, [FromBody] OrderDto orderDto)
        {
            var order = Orders.Find(o => o.Id == id && o.UserId == orderDto.UserId);
            if (order == null)
                return NotFound("Order not found.");

            if (!string.IsNullOrWhiteSpace(orderDto.ProductName))
                order.ProductName = orderDto.ProductName;
            order.Quantity = orderDto.Quantity;

            return Ok(order);
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="id">Order ID.</param>
        /// <param name="cancelDto">Cancel order data transfer object.</param>
        /// <returns>The canceled order or a not found/bad request result.</returns>
        [HttpPost]
        [Route("cancel/{id}")]
        public IActionResult CancelOrder(int id, [FromBody] CancelOrderDto cancelDto)
        {
            var order = Orders.Find(o => o.Id == id && o.UserId == cancelDto.UserId);
            if (order == null)
                return NotFound("Order not found.");

            if (order.Status == "Canceled")
                return BadRequest("Order is already canceled.");

            order.Status = "Canceled";
            return Ok(order);
        }
    }

    /// <summary>
    /// Data transfer object for creating or editing an order.
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Name of the product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// User ID who placed the order.
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// Data transfer object for canceling an order.
    /// </summary>
    public class CancelOrderDto
    {
        /// <summary>
        /// User ID who wants to cancel the order.
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// Order model.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Order ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the product.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Quantity of the product.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// User ID who placed the order.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Status of the order (e.g., Active, Canceled).
        /// </summary>
        public string Status { get; set; }
    }
}
