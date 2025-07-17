using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.Commands.ProcessOrderData;
using ReportingService.Domain.Events;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestDataController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TestDataController> _logger;

    public TestDataController(IMediator mediator, ILogger<TestDataController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generate and process sample order data for testing
    /// This endpoint is for development and testing purposes only
    /// </summary>
    /// <param name="orderCount">Number of sample orders to generate</param>
    /// <returns>Processing results</returns>
    [HttpPost("generate-sample-orders/{orderCount:int}")]
    public async Task<ActionResult> GenerateSampleOrders(int orderCount = 10)
    {
        try
        {
            if (orderCount > 100)
            {
                return BadRequest("Cannot generate more than 100 orders at once");
            }

            var results = new List<object>();
            var random = new Random();

            for (int i = 0; i < orderCount; i++)
            {
                // Generate sample order data
                var orderEvent = GenerateSampleOrderEvent(random);

                // Process the order
                var command = new ProcessOrderCompletedEventCommand
                {
                    OrderEvent = orderEvent
                };

                var result = await _mediator.Send(command);
                
                results.Add(new
                {
                    OrderId = orderEvent.OrderId,
                    Success = result.Success,
                    Message = result.Message,
                    OrderFactId = result.OrderFactId
                });
            }

            return Ok(new
            {
                Success = true,
                Message = $"Generated and processed {orderCount} sample orders",
                TotalOrders = orderCount,
                SuccessfulOrders = results.Count(r => ((dynamic)r).Success),
                Results = results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sample orders");
            return StatusCode(500, "An error occurred while generating sample orders");
        }
    }

    private static OrderCompletedEvent GenerateSampleOrderEvent(Random random)
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var orderDate = DateTime.Today.AddDays(-random.Next(0, 30)); // Random date in last 30 days

        var categories = new[] { "Electronics", "Clothing", "Books", "Home & Garden", "Sports" };
        var brands = new[] { "Apple", "Samsung", "Nike", "Adidas", "Sony", "LG", "Microsoft" };
        var products = new[]
        {
            ("iPhone 15", "Electronics", "Smartphones"),
            ("MacBook Pro", "Electronics", "Laptops"),
            ("Running Shoes", "Sports", "Footwear"),
            ("T-Shirt", "Clothing", "Casual"),
            ("Coffee Maker", "Home & Garden", "Kitchen"),
            ("Gaming Headset", "Electronics", "Accessories"),
            ("Yoga Mat", "Sports", "Fitness"),
            ("Jeans", "Clothing", "Casual"),
            ("Wireless Mouse", "Electronics", "Accessories"),
            ("Garden Tools", "Home & Garden", "Tools")
        };

        var itemCount = random.Next(1, 5); // 1-4 items per order
        var items = new List<OrderItem>();
        decimal totalAmount = 0;

        for (int i = 0; i < itemCount; i++)
        {
            var product = products[random.Next(products.Length)];
            var price = decimal.Round((decimal)(random.NextDouble() * 500 + 10), 2); // $10-$510
            var quantity = random.Next(1, 4); // 1-3 quantity
            var itemTotal = price * quantity;

            items.Add(new OrderItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = product.Item1,
                Category = product.Item2,
                SubCategory = product.Item3,
                Brand = brands[random.Next(brands.Length)],
                Price = price,
                Quantity = quantity,
                Total = itemTotal
            });

            totalAmount += itemTotal;
        }

        var tax = decimal.Round(totalAmount * 0.08m, 2); // 8% tax
        var discount = random.NextDouble() > 0.7 ? decimal.Round(totalAmount * 0.1m, 2) : 0; // 30% chance of 10% discount

        var segments = new[] { "Premium", "Standard", "Basic" };
        var countries = new[] { "USA", "Canada", "UK", "Germany", "France", "Australia" };
        var cities = new[] { "New York", "Toronto", "London", "Berlin", "Paris", "Sydney" };

        return new OrderCompletedEvent
        {
            OrderId = orderId,
            CustomerId = customerId,
            OrderDate = orderDate,
            TotalAmount = totalAmount + tax - discount,
            Currency = "USD",
            Status = "Completed",
            TotalItems = items.Sum(x => x.Quantity),
            Tax = tax,
            Discount = discount,
            Items = items,
            Customer = new CustomerInfo
            {
                Email = $"customer{customerId:N}@example.com",
                FirstName = "Test",
                LastName = "Customer",
                Country = countries[random.Next(countries.Length)],
                City = cities[random.Next(cities.Length)],
                Segment = segments[random.Next(segments.Length)],
                RegistrationDate = DateTime.Today.AddDays(-random.Next(30, 365))
            }
        };
    }
}
