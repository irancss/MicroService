using BuildingBlocks.Core.Contracts;
using BuildingBlocks.Identity;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Events.Contracts;
using BuildingBlocks.Messaging.Events.Domains;
using BuildingBlocks.ServiceMesh.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BuildingBlocks.Samples
{
    /// <summary>
    /// [اصلاح شد] یک کنترلر نمونه که استفاده از بلوک‌های سازنده مختلف را نشان می‌دهد.
    /// این کنترلر برای تست و نمایش قابلیت‌ها مناسب است.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SampleController : ControllerBase
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceMeshHttpClient _serviceMeshClient;
        private readonly ILogger<SampleController> _logger;
        private readonly ICurrentUserService _currentUser;

        public SampleController(
            IEventBus eventBus,
            IServiceMeshHttpClient serviceMeshClient, // [اصلاح شد] تزریق IServiceMeshHttpClient
            ILogger<SampleController> logger,
            ICurrentUserService currentUser)
        {
            _eventBus = eventBus;
            _serviceMeshClient = serviceMeshClient;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("User {UserId} is creating an order for customer {CustomerEmail}", _currentUser.UserId, request.CustomerEmail);

            var orderId = new Random().Next(1000, 9999);
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = orderId,
                Amount = request.Amount,
                CustomerEmail = request.CustomerEmail,
                Items = request.Items.Select(i => new OrderItemData(i.ProductId, i.Quantity)).ToList()
            };

            await _eventBus.PublishAsync(orderEvent);
            _logger.LogInformation("Order {OrderId} created and OrderCreatedEvent published with CorrelationId {CorrelationId}", orderId, orderEvent.CorrelationId);

            return Ok(new { OrderId = orderId, Message = "Order creation process started." });
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation("Fetching product {ProductId} by user {UserId}", id, _currentUser.UserId);

            // [اصلاح شد] استفاده از ServiceMeshHttpClient که خواناتر و قدرتمندتر است.
            var product = await _serviceMeshClient.GetFromJsonAsync<Product>("product-service", $"/api/products/{id}");

            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost("payments")]
        [ProducesResponseType(typeof(PaymentResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            _logger.LogInformation("Processing payment for order {OrderId} initiated by user {UserId}", request.OrderId, _currentUser.UserId);

            var response = await _serviceMeshClient.PostAsJsonAsync("payment-service", "/api/payments", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Payment service failed for order {OrderId} with status {StatusCode}", request.OrderId, response.StatusCode);
                return StatusCode((int)response.StatusCode, "Payment processing failed.");
            }

            var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
            if (paymentResponse == null)
            {
                return StatusCode(500, "Invalid response from payment service.");
            }

            // [نکته کلیدی] بر اساس پاسخ، رویداد مناسب برای ادامه Saga منتشر می‌شود.
            if (paymentResponse.IsSuccessful)
            {
                await _eventBus.PublishAsync(new PaymentSucceededEvent { OrderId = request.OrderId, PaymentId = paymentResponse.PaymentId });
                _logger.LogInformation("Payment for order {OrderId} succeeded and PaymentSucceededEvent published.", request.OrderId);
            }
            else
            {
                await _eventBus.PublishAsync(new PaymentFailedEvent { OrderId = request.OrderId, Reason = paymentResponse.ErrorMessage ?? "Unknown error." });
                _logger.LogWarning("Payment for order {OrderId} failed and PaymentFailedEvent published. Reason: {Reason}", request.OrderId, paymentResponse.ErrorMessage);
            }

            return Ok(paymentResponse);
        }
    }

    #region DTOs
    public record CreateOrderRequest(decimal Amount, string CustomerEmail, List<OrderItem> Items);
    public record OrderItem(int ProductId, int Quantity);
    public record ProcessPaymentRequest(int OrderId, decimal Amount, string PaymentMethod, string CardNumber);
    public record PaymentResponse(string PaymentId, bool IsSuccessful, string? ErrorMessage);
    public record Product(int Id, string Name, decimal Price, string? Description);
    #endregion
}