using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using BuildingBlocks.ServiceDiscovery;
using BuildingBlocks.Messaging;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.ServiceMesh.Http;
using BuildingBlocks.Resiliency;
using BuildingBlocks.Observability;
using BuildingBlocks.Identity;
using MassTransit;
using System.Diagnostics;

namespace BuildingBlocks.Samples
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceMeshHttpClient _httpClient;
        private readonly ILogger<SampleController> _logger;

        public SampleController(
            IMessageBus messageBus, 
            IServiceMeshHttpClient httpClient,
            ILogger<SampleController> logger)
        {
            _messageBus = messageBus;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Example of publishing an event using MassTransit
        /// </summary>
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("Creating order for customer {CustomerEmail}", request.CustomerEmail);

            // Simulate order creation logic
            var orderId = new Random().Next(1000, 9999);

            // Publish OrderCreatedEvent
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = orderId,
                Amount = request.Amount,
                CustomerEmail = request.CustomerEmail,
                OrderDate = DateTime.UtcNow
            };

            await _messageBus.PublishAsync(orderEvent);

            _logger.LogInformation("Order {OrderId} created and event published", orderId);

            return Ok(new { OrderId = orderId, Message = "Order created successfully" });
        }

        /// <summary>
        /// Example of service-to-service communication using Service Mesh
        /// </summary>
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation("Fetching product {ProductId} from product service", id);

            try
            {
                // Call product service through service mesh
                var product = await _httpClient.GetFromJsonAsync<Product>("product-service", $"/api/products/{id}");

                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found", id);
                    return NotFound($"Product {id} not found");
                }

                _logger.LogInformation("Product {ProductId} retrieved successfully", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product {ProductId}", id);
                return StatusCode(500, "Error fetching product");
            }
        }

        /// <summary>
        /// Example of calling multiple services
        /// </summary>
        [HttpGet("order-details/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            _logger.LogInformation("Fetching order details for {OrderId}", orderId);

            try
            {
                // Call multiple services in parallel
                var orderTask = _httpClient.GetFromJsonAsync<Order>("order-service", $"/api/orders/{orderId}");
                var customerTask = _httpClient.GetFromJsonAsync<Customer>("customer-service", $"/api/customers/by-order/{orderId}");

                await Task.WhenAll(orderTask, customerTask);

                var order = await orderTask;
                var customer = await customerTask;

                if (order == null || customer == null)
                {
                    return NotFound("Order or customer not found");
                }

                var orderDetails = new OrderDetails
                {
                    Order = order,
                    Customer = customer
                };

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order details for {OrderId}", orderId);
                return StatusCode(500, "Error fetching order details");
            }
        }

        /// <summary>
        /// Example of sending a command to a specific service
        /// </summary>
        [HttpPost("payments")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            _logger.LogInformation("Processing payment for order {OrderId}", request.OrderId);

            try
            {
                // Send payment command to payment service
                var paymentResponse = await _httpClient.PostAsJsonAsync(
                    "payment-service", 
                    "/api/payments/process", 
                    request);

                if (paymentResponse == null || !paymentResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500, "Payment service unavailable");
                }

                // Publish PaymentProcessedEvent
                var paymentEvent = new PaymentProcessedEvent
                {
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    IsSuccessful = paymentResponse.IsSuccessStatusCode,
                    PaymentId = Guid.NewGuid().ToString()
                };

                await _messageBus.PublishAsync(paymentEvent);

                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", request.OrderId);
                return StatusCode(500, "Error processing payment");
            }
        }
    }

    // DTOs
    public class CreateOrderRequest
    {
        public decimal Amount { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProcessPaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
    }

    public class PaymentResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class OrderDetails
    {
        public Order Order { get; set; } = new();
        public Customer Customer { get; set; } = new();
    }
}
