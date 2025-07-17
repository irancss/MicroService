# Setup Guide for BuildingBlocks

This guide will help you set up and run the BuildingBlocks components in your development environment.

## Prerequisites

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio Code or Visual Studio

## Infrastructure Setup

### 1. RabbitMQ Setup

Run RabbitMQ using Docker:

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin123 \
  rabbitmq:3-management
```

**Access RabbitMQ Management UI:**
- URL: http://localhost:15672
- Username: admin
- Password: admin123

### 2. Consul Setup

Run Consul using Docker:

```bash
docker run -d --name consul \
  -p 8500:8500 \
  -p 8600:8600/udp \
  consul:latest agent -server -ui -node=server-1 -bootstrap-expect=1 -client=0.0.0.0
```

**Access Consul UI:**
- URL: http://localhost:8500

### 3. Docker Compose (Alternative)

Create a `docker-compose.yml` file:

```yaml
version: '3.8'
services:
  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: admin123
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq

  consul:
    image: consul:latest
    container_name: consul
    ports:
      - "8500:8500"
      - "8600:8600/udp"
    command: agent -server -ui -node=server-1 -bootstrap-expect=1 -client=0.0.0.0
    volumes:
      - consul_data:/consul/data

volumes:
  rabbitmq_data:
  consul_data:
```

Run with: `docker-compose up -d`

## Project Configuration

### 1. Update appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "Username": "admin",
    "Password": "admin123"
  },
  "Consul": {
    "Host": "localhost",
    "Port": 8500,
    "Datacenter": "dc1"
  },
  "ServiceMesh": {
    "ServiceName": "building-blocks-sample",
    "ServiceId": "building-blocks-sample-001",
    "Address": "localhost",
    "Port": 5000,
    "Tags": ["api", "sample", "v1"],
    "EnableHealthCheck": true,
    "EnableLoadBalancing": true,
    "EnableCircuitBreaker": true
  },
  "JWT": {
    "Issuer": "https://localhost:5000",
    "Audience": "https://localhost:5000",
    "SecretKey": "this-is-a-super-secret-key-that-should-be-at-least-256-bits-long",
    "ExpirationMinutes": 60
  },
  "ApiGateway": {
    "BaseUrl": "https://localhost:5000",
    "Port": 5000,
    "EnableRateLimiting": true,
    "RateLimitRequestsPerMinute": 100,
    "EnableLogging": true
  },
  "ConnectionStrings": {
    "RabbitMQ": "amqp://admin:admin123@localhost:5672/"
  },
  "AllowedHosts": "*"
}
```

### 2. Create a Test Application

Create a new ASP.NET Core Web API project:

```bash
dotnet new webapi -n TestMicroservice
cd TestMicroservice
dotnet add reference ../BuildingBlocks/BuildingBlocks.csproj
```

### 3. Update Program.cs

```csharp
using BuildingBlocks;
using BuildingBlocks.Messaging.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Option 1: Add BuildingBlocks with basic messaging
builder.Services.AddBuildingBlocks(builder.Configuration);

// Option 2: Add BuildingBlocks with full Event-Driven Architecture
builder.Services.AddBuildingBlocksWithEventDriven(builder.Configuration, 
    typeof(OrderCreatedEventHandler),
    typeof(ProductEventHandler),
    typeof(CustomerEventHandler));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Use BuildingBlocks
app.UseBuildingBlocks();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## Testing the Components

### 1. Test Messaging

Create an order to test event publishing:

```bash
curl -X POST "https://localhost:5001/api/sample/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 100.50,
    "customerEmail": "test@example.com",
    "items": [
      {
        "productId": 1,
        "quantity": 2,
        "price": 50.25
      }
    ]
  }'
```

### 2. Test Service Discovery

Check Consul for registered services:
- Go to http://localhost:8500
- Check the "Services" tab
- You should see your service registered

### 3. Test Health Checks

```bash
# Overall health
curl https://localhost:5001/health

# Readiness check
curl https://localhost:5001/health/ready

# Liveness check
curl https://localhost:5001/health/live
```

### 4. Test Circuit Breaker

The circuit breaker will automatically open when there are failures. You can test this by:
1. Stopping a downstream service
2. Making multiple requests
3. Observing the circuit breaker behavior in logs

## Monitoring and Observability

### 1. RabbitMQ Monitoring
- Management UI: http://localhost:15672
- Monitor queues, exchanges, and message rates

### 2. Consul Monitoring
- UI: http://localhost:8500
- Monitor service health and registrations

### 3. Application Logs
- Use structured logging
- Include correlation IDs for request tracing
- Monitor circuit breaker state changes

## Production Considerations

### 1. Security
- Use strong JWT secrets
- Enable HTTPS in production
- Configure proper CORS policies
- Use authentication for RabbitMQ and Consul

### 2. Scalability
- Use multiple RabbitMQ nodes for clustering
- Set up Consul cluster for high availability
- Configure load balancing properly

### 3. Monitoring
- Add application performance monitoring (APM)
- Set up alerts for circuit breaker states
- Monitor message queue depths

### 4. Configuration
- Use Azure Key Vault or similar for secrets
- Environment-specific configurations
- Configuration validation on startup

## Troubleshooting

### Common Issues

1. **Service not registering with Consul**
   - Check Consul is running
   - Verify network connectivity
   - Check service configuration

2. **Messages not being processed**
   - Verify RabbitMQ is running
   - Check queue declarations
   - Verify consumer registration

3. **Circuit breaker always open**
   - Check downstream service health
   - Verify timeout configurations
   - Review failure thresholds

4. **API Gateway routing issues**
   - Verify ocelot.json configuration
   - Check service discovery settings
   - Ensure downstream services are healthy

### Logs to Check

- Application startup logs
- Service registration logs
- Message processing logs
- HTTP request/response logs
- Circuit breaker state changes

For additional help, check the README.md file in the BuildingBlocks project.

## Event-Driven Communication

The BuildingBlocks library provides comprehensive Event-Driven Communication capabilities:

### 1. Publishing Events

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IEventBus _eventBus;

    public OrderController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Business logic here...
        var orderId = new Random().Next(1000, 9999);

        // Publish domain event
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = orderId,
            Amount = request.Amount,
            CustomerEmail = request.CustomerEmail,
            OrderDate = DateTime.UtcNow
        });

        return Ok(new { OrderId = orderId });
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request)
    {
        // Business logic here...

        // Publish cancellation event
        await _eventBus.PublishAsync(new OrderCancelledEvent
        {
            OrderId = id,
            CustomerEmail = request.CustomerEmail,
            RefundAmount = request.RefundAmount,
            CancellationReason = request.Reason
        });

        return Ok();
    }
}
```

### 2. Event Handlers

```csharp
public class OrderEventHandler : 
    IEventHandler<OrderCreatedEvent>,
    IEventHandler<OrderCancelledEvent>
{
    private readonly ILogger<OrderEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public OrderEventHandler(ILogger<OrderEventHandler> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;
        
        _logger.LogInformation("Processing order creation for Order {OrderId}", orderEvent.OrderId);

        // Send welcome email
        await _notificationService.SendEmailAsync(
            orderEvent.CustomerEmail,
            "Order Confirmation",
            $"Your order {orderEvent.OrderId} has been created successfully."
        );

        // Trigger inventory reservation
        await context.Publish(new InventoryReservationRequestedEvent
        {
            OrderId = orderEvent.OrderId,
            // ... other properties
        });
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var cancelEvent = context.Message;
        
        _logger.LogInformation("Processing order cancellation for Order {OrderId}", cancelEvent.OrderId);

        // Send cancellation notification
        await _notificationService.SendEmailAsync(
            cancelEvent.CustomerEmail,
            "Order Cancellation",
            $"Your order {cancelEvent.OrderId} has been cancelled. Refund: ${cancelEvent.RefundAmount}"
        );
    }
}
```

### 3. Available Domain Events

The library includes pre-built domain events for common e-commerce scenarios:

#### Order Events
- `OrderCreatedEvent` - When a new order is placed
- `OrderStatusChangedEvent` - When order status changes (confirmed, shipped, delivered)
- `OrderCancelledEvent` - When an order is cancelled

#### Product Events  
- `ProductCreatedEvent` - When a new product is added
- `ProductUpdatedEvent` - When product details change
- `StockUpdatedEvent` - When inventory levels change

#### Payment Events
- `PaymentProcessedEvent` - When payment is processed (success/failure)

#### Customer Events
- `CustomerRegisteredEvent` - When a new customer registers
- `UserLoginEvent` / `UserLogoutEvent` - User session events

#### Shipment Events
- `ShipmentCreatedEvent` - When shipment is created
- `ShipmentStatusChangedEvent` - When shipment status updates

#### Notification Events
- `NotificationRequestedEvent` - Request to send notification
- `NotificationSentEvent` - Notification delivery confirmation

#### Analytics Events
- `ProductViewedEvent` - When products are viewed
- `CartUpdatedEvent` - When shopping cart changes

### 4. Event-Driven Sagas

For complex business workflows, use sagas to coordinate multiple services:

```csharp
public class OrderProcessingSaga : SagaOrchestrator<OrderProcessingData>,
    IConsumer<OrderCreatedEvent>,
    IConsumer<PaymentProcessedEvent>,
    IConsumer<InventoryReservedEvent>
{
    public OrderProcessingSaga(IEventBus eventBus) : base(eventBus)
    {
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;
        
        Data.OrderId = orderEvent.OrderId;
        Data.CurrentState = "OrderCreated";

        // Request inventory reservation
        await PublishEventAsync(new InventoryReservationRequestedEvent
        {
            OrderId = orderEvent.OrderId,
            // ... other properties
        });
    }

    // Handle other events...
}
```

### 5. Event Store and Audit Trail

All events are automatically stored for audit purposes:

```csharp
public class OrderService
{
    private readonly IEventStore _eventStore;

    public async Task<IEnumerable<BaseEvent>> GetOrderHistoryAsync(int orderId)
    {
        return await _eventStore.GetEventsAsync("OrderCreatedEvent", 
            DateTime.Today.AddDays(-30), DateTime.Now);
    }
}
```

### 6. Testing Event-Driven Components

```bash
# Test order creation and event publishing
curl -X POST "https://localhost:5001/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 150.00,
    "customerEmail": "customer@example.com",
    "items": [
      {
        "productId": 1,
        "quantity": 2,
        "price": 75.00
      }
    ]
  }'

# Test product update event
curl -X PUT "https://localhost:5001/api/products/1" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Product",
    "price": 99.99,
    "stockQuantity": 50
  }'
```

### 7. Monitoring Events

Check RabbitMQ Management UI to monitor:
- Queue depths
- Message rates
- Consumer status
- Failed messages

Access: http://localhost:15672
- Username: admin
- Password: admin123

### 8. Event-Driven Best Practices

1. **Event Naming**: Use past tense (OrderCreated, PaymentProcessed)
2. **Event Size**: Keep events small and focused
3. **Idempotency**: Ensure handlers are idempotent
4. **Error Handling**: Implement retry policies and dead letter queues
5. **Versioning**: Plan for event schema evolution
6. **Correlation IDs**: Use correlation IDs for tracing across services
