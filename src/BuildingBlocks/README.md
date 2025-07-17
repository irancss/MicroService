# .NET Microservices Building Blocks

A comprehensive collection of building blocks for developing .NET microservices with industry best practices.

## 🏗️ Architecture Overview

This library provides reusable components for:

1. **Service Discovery** (Consul)
2. **API Gateway** (YARP) 
3. **Asynchronous Communication** (RabbitMQ + MassTransit)
4. **Centralized Configuration** (Consul KV)
5. **Resiliency** (Polly)
6. **Distributed Tracing & Observability** (OpenTelemetry, Jaeger, Serilog, Seq, Prometheus)
7. **Identity & Access Management** (Duende IdentityServer)

## 🚀 Quick Start

### 1. Service Discovery with Consul

Register and discover services dynamically:

```csharp
// In Program.cs
builder.Services.AddConsulServiceDiscovery(builder.Configuration);

// Use service discovery
public class ProductController : ControllerBase
{
    private readonly IConsulServiceDiscovery _serviceDiscovery;
    
    public async Task<IActionResult> GetInventory()
    {
        var services = await _serviceDiscovery.DiscoverServicesAsync("inventory-service");
        // Use discovered service instances
    }
}
```

**Configuration (appsettings.json):**
```json
{
  "Consul": {
    "Address": "http://localhost:8500",
    "ServiceName": "product-service",
    "ServiceId": "product-service-1",
    "Port": 5001,
    "Tags": ["product", "api"],
    "HealthCheckInterval": "00:00:30"
  }
}
```

### 2. API Gateway with YARP

Centralized routing and aggregation:

```csharp
// In API Gateway Program.cs
builder.Services.AddYarpApiGateway(builder.Configuration);

var app = builder.Build();
app.MapReverseProxy();

// Custom aggregation endpoint
app.MapGet("/api/aggregate/dashboard", async (IServiceProvider sp) =>
{
    // Aggregate data from multiple microservices
    return Results.Ok(new { Orders = "...", Products = "...", Users = "..." });
});
```

**YARP Configuration:**
```json
{
  "ReverseProxy": {
    "Routes": {
      "products": {
        "ClusterId": "product-cluster",
        "Match": {
          "Path": "/api/products/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "product-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5001/"
          }
        }
      }
    }
  }
}
```

### 3. Asynchronous Communication with MassTransit

Event-driven communication between services:

```csharp
// Define events
public class OrderCreatedEvent : BaseEvent
{
    public string OrderId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
}

// Publisher (Order Service)
public class OrderController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
    {
        // Create order logic...
        
        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = orderId,
            UserId = request.UserId,
            Amount = request.Amount
        });
        
        return Ok();
    }
}

// Consumer (Inventory Service)
public class OrderCreatedEventHandler : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        // Reserve inventory logic...
        
        await context.Publish(new InventoryReservedEvent
        {
            OrderId = order.OrderId,
            // ... other properties
        });
    }
}

// Configuration in Program.cs
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventHandler>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

### 4. Centralized Configuration with Consul KV

Store configuration in Consul and load dynamically:

```csharp
// In Program.cs
builder.Configuration.AddConsul("microservices/product-service", options =>
{
    options.ConsulConfigurationOptions = config =>
    {
        config.Address = new Uri("http://localhost:8500");
    };
    options.Optional = true;
    options.ReloadOnChange = true;
});

// Store in Consul KV:
// Key: microservices/product-service/ConnectionStrings/DefaultConnection
// Value: "Server=localhost;Database=ProductDb;..."
```

### 5. Resiliency with Polly

Implement retry, circuit breaker, and timeout policies:

```csharp
// Configure resilient HTTP client
builder.Services.AddHttpClient<IProductService, ProductService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30));
}
```

### 6. Observability

#### Distributed Tracing with OpenTelemetry
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter());
```

#### Structured Logging with Serilog
```csharp
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.WithProperty("Service", "ProductService")
          .WriteTo.Console()
          .WriteTo.Seq("http://localhost:5341");
});
```

#### Metrics with Prometheus
```csharp
builder.Services.AddSingleton<ICustomMetrics, CustomMetrics>();

// In controller
_customMetrics.IncrementCounter("orders_created", 
    new KeyValuePair<string, object?>("product", "laptop"));
```

### 7. Identity & Access Management

#### Identity Server Setup
```csharp
// Identity Server
builder.Services.AddCustomIdentityServer(builder.Configuration);

var app = builder.Build();
app.UseIdentityServer();
```

#### JWT Authentication in Services
```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);

// In controller
[Authorize(Policy = "ApiScope")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAll() => Ok(products);
}
```

```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "Username": "guest",
    "Password": "guest"
  },
  "Consul": {
    "Host": "localhost",
    "Port": 8500,
    "Datacenter": "dc1"
  },
  "ServiceMesh": {
    "ServiceName": "my-service",
    "ServiceId": "my-service-001",
    "Address": "localhost",
    "Port": 5000,
    "Tags": ["api", "v1"]
  },
  "JWT": {
    "Issuer": "https://your-issuer.com",
    "Audience": "https://your-audience.com",
    "SecretKey": "your-super-secret-key-that-should-be-at-least-256-bits",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@localhost:5672/"
  }
}
```

### 3. Startup Configuration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Option 1: Basic BuildingBlocks
        services.AddBuildingBlocks(Configuration);
        
        // Option 2: Full Event-Driven Architecture
        services.AddBuildingBlocksWithEventDriven(Configuration, 
            typeof(OrderCreatedEventHandler),
            typeof(ProductEventHandler));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Use BuildingBlocks middleware
        app.UseBuildingBlocks();
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

### 4. Publishing Events

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public OrdersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Create order logic here...
        
        // Publish event
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = 123,
            Amount = request.Amount,
            CustomerEmail = request.CustomerEmail,
            OrderDate = DateTime.UtcNow
        };
        
        await _messageBus.PublishAsync(orderEvent);
        
        return Ok();
    }
}
```

### 4. Event-Driven Usage

```csharp
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IEventBus _eventBus;

    public OrdersController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Create order logic here...
        
        // Publish domain event
        await _eventBus.PublishAsync(new OrderCreatedEvent
        {
            OrderId = 123,
            Amount = request.Amount,
            CustomerEmail = request.CustomerEmail,
            OrderDate = DateTime.UtcNow
        });
        
        return Ok();
    }
}
```

### 5. Event Handlers

```csharp
public class OrderEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderEventHandler> _logger;

    public OrderEventHandler(ILogger<OrderEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;
        
        _logger.LogInformation("Order {OrderId} created for {CustomerEmail}", 
            orderEvent.OrderId, orderEvent.CustomerEmail);

        // Handle the event (send notification, update inventory, etc.)
        await Task.CompletedTask;
    }
}
```

### 5. Service-to-Service Communication

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IServiceMeshHttpClient _httpClient;

    public ProductsController(IServiceMeshHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        // Call another service through service mesh
        var product = await _httpClient.GetAsync<Product>("product-service", $"/api/products/{id}");
        
        if (product == null)
            return NotFound();
            
        return Ok(product);
    }
}
```

## API Gateway Configuration

Create an `ocelot.json` file for API Gateway configuration:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/orders/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 5001
        }
      ],
      "UpstreamPathTemplate": "/api/orders/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ],
      "ServiceName": "order-service",
      "LoadBalancerOptions": {
        "Type": "RoundRobin"
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1m",
        "Limit": 100
      }
    }
  ],
  "GlobalConfiguration": {
    "ServiceDiscoveryProvider": {
      "Host": "localhost",
      "Port": 8500,
      "Type": "Consul"
    }
  }
}
```

## Docker Setup

### RabbitMQ
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Consul
```bash
docker run -d --name consul -p 8500:8500 consul:latest agent -dev -client=0.0.0.0
```

## Health Checks

The following health check endpoints are available:

- `/health` - Overall application health
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

## Features

### Messaging
- ✅ Event-driven architecture with MassTransit
- ✅ RabbitMQ integration
- ✅ Retry policies
- ✅ Circuit breaker for messaging

### API Gateway
- ✅ Request routing with Ocelot
- ✅ JWT authentication
- ✅ Rate limiting
- ✅ Request logging
- ✅ CORS support

### Service Mesh
- ✅ Service discovery with Consul
- ✅ Load balancing (Round-robin, Random, Weighted)
- ✅ Circuit breaker pattern
- ✅ Service-to-service communication
- ✅ Health checks integration

### Resilience
- ✅ Circuit breaker
- ✅ Retry policies
- ✅ Timeout handling
- ✅ Health monitoring

## Best Practices

1. **Configuration**: Use strongly-typed configuration classes
2. **Logging**: Implement structured logging with correlation IDs
3. **Error Handling**: Use consistent error handling across services
4. **Security**: Always validate JWT tokens and implement proper CORS
5. **Monitoring**: Use health checks and metrics for observability
6. **Testing**: Write integration tests for message handlers and HTTP clients

## Contributing

1. Follow the existing code style
2. Add unit tests for new features
3. Update documentation
4. Create pull requests for review

## License

This project is licensed under the MIT License.
