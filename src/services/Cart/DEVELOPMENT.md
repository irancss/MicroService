# Development Setup Guide

## Quick Start

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code
- Git

### Option 1: Full Docker Environment

```bash
# Clone and navigate to the cart service
cd src/services/Cart

# Start all services with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f cart-api

# Access services:
# - Cart API: https://localhost:5001
# - Swagger UI: https://localhost:5001/swagger
# - Hangfire Dashboard: https://localhost:5001/hangfire
# - Redis Commander: http://localhost:8081
# - pgAdmin: http://localhost:8080 (admin@example.com / admin)
# - RabbitMQ Management: http://localhost:15672 (guest / guest)
```

### Option 2: Local Development with External Dependencies

```bash
# Start only external dependencies
docker-compose up -d redis postgres rabbitmq

# Run the API locally
dotnet run --project CartService.API

# Or with hot reload
dotnet watch run --project CartService.API
```

## Development Workflow

### 1. Project Structure
```
Cart/
├── Cart.Domain/                    # Core business logic
│   ├── Entities/
│   ├── Enums/
│   ├── Events/
│   └── ValueObjects/
├── CartService.Application/        # CQRS handlers & DTOs
│   ├── Commands/
│   ├── Queries/
│   ├── Handlers/
│   ├── DTOs/
│   ├── Validators/
│   └── Behaviors/
├── CartService.Infrastructure/     # External concerns
│   ├── Repositories/
│   ├── Services/
│   ├── GrpcClients/
│   └── BackgroundJobs/
├── CartService.API/               # Web API layer
│   └── Controllers/
├── Dockerfile
├── docker-compose.yml
└── README.md
```

### 2. Testing the Dual-Cart Features

#### Test Scenario 1: Next-Purchase Auto-Activation
```bash
# Using curl or Postman

# 1. Add item to next-purchase cart
curl -X POST "https://localhost:5001/api/v1/carts/next-purchase/items" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user-123",
    "productId": "prod-laptop",
    "quantity": 1
  }'

# 2. Verify next-purchase cart has items
curl "https://localhost:5001/api/v1/carts?userId=test-user-123"

# 3. Add item to active cart (triggers auto-activation)
curl -X POST "https://localhost:5001/api/v1/carts/items" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user-123",
    "productId": "prod-phone",
    "quantity": 1
  }'

# 4. Check response for nextPurchaseItemsActivated: true
# and activationMessage: "ما محصولاتی که برای خرید بعدی ذخیره کرده بودید را به سبد خریدتان اضافه کردیم!"
```

#### Test Scenario 2: Item Movement
```bash
# Move item from active to next-purchase
curl -X POST "https://localhost:5001/api/v1/carts/items/prod-laptop/move-to-next-purchase" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user-123",
    "quantity": 1
  }'

# Move item from next-purchase to active
curl -X POST "https://localhost:5001/api/v1/carts/items/prod-laptop/move-to-active" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user-123"
  }'
```

### 3. Configuration Testing

#### Toggle Auto-Activation
```bash
# Disable auto-activation
curl -X PATCH "https://localhost:5001/api/v1/admin/cart-config/auto-activate-next-purchase" \
  -H "Content-Type: application/json" \
  -d '{"enabled": false}'

# Test that next-purchase items are NOT auto-activated
# (Repeat Test Scenario 1 and verify no auto-activation)

# Re-enable auto-activation
curl -X PATCH "https://localhost:5001/api/v1/admin/cart-config/auto-activate-next-purchase" \
  -H "Content-Type: application/json" \
  -d '{"enabled": true}'
```

#### Update Abandonment Settings
```bash
curl -X PATCH "https://localhost:5001/api/v1/admin/cart-config/abandonment-settings" \
  -H "Content-Type: application/json" \
  -d '{
    "thresholdMinutes": 15,
    "emailEnabled": true,
    "smsEnabled": false,
    "maxNotifications": 2
  }'
```

### 4. Background Jobs Testing

#### Monitoring Hangfire
1. Open Hangfire Dashboard: https://localhost:5001/hangfire
2. Check scheduled jobs:
   - `process-abandoned-carts` (every 30 minutes)
   - `move-abandoned-to-next-purchase` (daily at 2 AM)
   - `cleanup-expired-carts` (weekly on Sunday at 3 AM)

#### Manual Job Execution
```bash
# Trigger abandonment job manually via Hangfire dashboard
# Or create abandoned cart scenario:

# 1. Add items to active cart
curl -X POST "https://localhost:5001/api/v1/carts/items" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-abandon-user",
    "productId": "prod-expensive",
    "quantity": 1
  }'

# 2. Wait for abandonment threshold (default 30 minutes)
# Or modify LastModifiedUtc in Redis manually

# 3. Run abandonment job and check logs
```

## Common Development Tasks

### Adding a New Command/Query

1. **Create Command/Query**
```csharp
// In CartService.Application/Commands/
public class YourNewCommand : IRequest<CartOperationResult>
{
    public string UserId { get; set; }
    // ... other properties
}
```

2. **Create Handler**
```csharp
// In CartService.Application/Handlers/Commands/
public class YourNewCommandHandler : IRequestHandler<YourNewCommand, CartOperationResult>
{
    // Implement Handle method
}
```

3. **Add Validator (if needed)**
```csharp
// In CartService.Application/Validators/
public class YourNewCommandValidator : AbstractValidator<YourNewCommand>
{
    // Add validation rules
}
```

4. **Add Controller Endpoint**
```csharp
// In CartService.API/Controllers/CartController.cs
[HttpPost("your-endpoint")]
public async Task<ActionResult<CartOperationResult>> YourEndpoint([FromBody] YourRequest request)
{
    var command = new YourNewCommand { /* map properties */ };
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### Debugging Tips

#### Redis Data Inspection
```bash
# Connect to Redis
docker exec -it cart-redis redis-cli

# List all cart keys
KEYS cart:*

# Get specific cart
GET cart:user:test-user-123

# Delete cart (for testing)
DEL cart:user:test-user-123
```

#### Log Analysis
```bash
# View real-time logs
docker-compose logs -f cart-api

# View Hangfire logs
docker-compose logs -f cart-api | grep -i hangfire

# View specific component logs
docker-compose logs -f cart-api | grep -i "CartAbandonmentJob"
```

#### Event Monitoring
```bash
# RabbitMQ Management UI
open http://localhost:15672

# Check exchanges and queues
# Monitor message rates and consumer activity
```

## Performance Testing

### Load Testing with curl
```bash
# Simple load test script
for i in {1..100}; do
  curl -X POST "https://localhost:5001/api/v1/carts/items" \
    -H "Content-Type: application/json" \
    -d "{\"userId\":\"load-test-user-$i\",\"productId\":\"prod-test\",\"quantity\":1}" &
done
wait
```

### Redis Performance Monitoring
```bash
# Monitor Redis performance
docker exec -it cart-redis redis-cli --latency-history

# Memory usage
docker exec -it cart-redis redis-cli INFO memory
```

## Deployment Checklist

### Before Production
- [ ] Update connection strings in appsettings.Production.json
- [ ] Configure proper authentication/authorization
- [ ] Set up SSL certificates
- [ ] Configure proper CORS policies
- [ ] Set up monitoring and alerting
- [ ] Review and test backup strategies
- [ ] Load test with production-like data
- [ ] Security scan and vulnerability assessment

### Environment Variables
```bash
# Required environment variables for production
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Redis=your-redis-connection
ConnectionStrings__Hangfire=your-postgres-connection
RabbitMQ__Host=your-rabbitmq-host
RabbitMQ__Username=your-username
RabbitMQ__Password=your-password
GrpcSettings__InventoryServiceUrl=your-inventory-service
GrpcSettings__CatalogServiceUrl=your-catalog-service
```

## Troubleshooting

### Common Issues

#### 1. Redis Connection Issues
```bash
# Check Redis connectivity
docker exec -it cart-redis redis-cli ping

# Restart Redis
docker-compose restart redis
```

#### 2. Hangfire Jobs Not Running
```bash
# Check Hangfire dashboard
open https://localhost:5001/hangfire

# Check PostgreSQL connection
docker exec -it cart-postgres psql -U postgres -d cart_hangfire -c "\dt"
```

#### 3. gRPC Service Unavailable
```bash
# Check if external services are running
curl https://localhost:5001/health

# Mock the services if needed during development
```

#### 4. High Memory Usage
```bash
# Check Redis memory usage
docker exec -it cart-redis redis-cli INFO memory

# Consider adjusting maxmemory policy in docker-compose.yml
```

### Debug Mode
```bash
# Run with debug logging
export ASPNETCORE_ENVIRONMENT=Development
export Logging__LogLevel__Default=Debug
dotnet run --project CartService.API
```

This setup guide should get you up and running with the dual-cart microservice quickly and provide guidance for ongoing development and testing.
