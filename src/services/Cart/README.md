# Cart Microservice - Dual-Cart E-commerce Solution

## Overview

This is a sophisticated, dual-cart microservice built with Clean Architecture principles, implementing CQRS pattern with MediatR. The service manages both Active Cart and Next-Time Purchase Cart functionality with intelligent features designed to boost customer retention and average order value.

## Architecture

### Clean Architecture Layers

1. **Domain Layer** (`Cart.Domain`)
   - Core entities: `ShoppingCart`, `CartItem`
   - Domain events and enums
   - Value objects and configuration

2. **Application Layer** (`Cart.Application`)
   - CQRS Commands and Queries
   - MediatR handlers
   - DTOs and interfaces
   - Business logic and validation

3. **Infrastructure Layer** (`Cart.Infrastructure`)
   - Redis repository implementation
   - gRPC clients for external services
   - Hangfire background jobs
   - Event publishing with RabbitMQ/MassTransit

4. **API Layer** (`Cart.API`)
   - RESTful controllers
   - Swagger documentation
   - Authentication and authorization

## Key Features

### Dual-Cart System

- **Active Cart**: Standard shopping cart for current session items
- **Next-Time Purchase Cart**: Persistent list for future purchases (not just a wishlist)

### Intelligent Next-Purchase Activation

When a user with items in their Next-Time Purchase Cart returns and adds their first item to an empty Active Cart, the system automatically moves all Next-Time Purchase items to the Active Cart with a notification.

### Dynamic Configuration

Administrators can configure:
- Auto-activation of next-purchase items
- Abandonment thresholds and actions
- Notification settings
- Real-time validation preferences

### Advanced Abandonment Recovery

- Multi-step, multi-channel campaigns (email, SMS)
- Configurable actions (move to Next-Time Purchase after X days)
- Smart notifications with discount integration

## Technology Stack

- **.NET 8**: Latest framework with performance improvements
- **Redis**: High-performance caching and cart storage
- **PostgreSQL**: Hangfire job storage
- **Hangfire**: Background job processing
- **RabbitMQ + MassTransit**: Event-driven messaging
- **gRPC**: High-performance service communication
- **Serilog**: Structured logging
- **FluentValidation**: Request validation
- **Swagger/OpenAPI**: API documentation

## API Endpoints

### Cart Operations

- `GET /api/v1/carts` - Get user's complete cart
- `GET /api/v1/carts/summary` - Get cart summary
- `POST /api/v1/carts/items` - Add item to active cart
- `POST /api/v1/carts/next-purchase/items` - Add item to next-purchase cart
- `POST /api/v1/carts/items/{productId}/move-to-next-purchase` - Move item to next-purchase
- `POST /api/v1/carts/items/{productId}/move-to-active` - Move item to active cart
- `PUT /api/v1/carts/items/{productId}` - Update item quantity
- `DELETE /api/v1/carts/items/{productId}` - Remove item
- `DELETE /api/v1/carts/clear` - Clear cart
- `POST /api/v1/carts/merge` - Merge guest cart with user cart
- `POST /api/v1/carts/activate-next-purchase` - Manually activate next-purchase items

### Configuration Management

- `GET /api/v1/admin/cart-config` - Get configuration
- `PUT /api/v1/admin/cart-config` - Update configuration
- `PATCH /api/v1/admin/cart-config/auto-activate-next-purchase` - Toggle auto-activation
- `PATCH /api/v1/admin/cart-config/abandonment-settings` - Update abandonment settings

## Data Model

### Redis Storage Structure

```json
{
  "UserId": "user-123",
  "LastModifiedUtc": "2023-10-27T10:00:00Z",
  "ActiveItems": [
    {
      "ProductId": "prod-A",
      "Quantity": 1,
      "PriceAtTimeOfAddition": 500,
      "ProductName": "Product A",
      "AddedUtc": "2023-10-27T09:30:00Z"
    }
  ],
  "NextPurchaseItems": [
    {
      "ProductId": "prod-C",
      "Quantity": 1,
      "PriceAtTimeOfAddition": 1200,
      "ProductName": "Product C",
      "AddedUtc": "2023-10-26T15:00:00Z"
    }
  ]
}
```

## Configuration

### Environment Variables

- `ConnectionStrings__Redis`: Redis connection string
- `ConnectionStrings__Hangfire`: PostgreSQL connection for Hangfire
- `RabbitMQ__Host`: RabbitMQ host
- `GrpcSettings__InventoryServiceUrl`: Inventory service gRPC endpoint
- `GrpcSettings__CatalogServiceUrl`: Catalog service gRPC endpoint

### Default Configuration

- Auto next-purchase activation: Enabled
- Abandonment threshold: 30 minutes
- Move to next-purchase after: 7 days
- Max abandonment notifications: 3
- Notification interval: 24 hours

## Background Jobs

1. **Process Abandoned Carts** (Every 30 minutes)
   - Detects abandoned carts
   - Sends email/SMS notifications
   - Schedules follow-up notifications

2. **Move Abandoned to Next-Purchase** (Daily at 2 AM)
   - Moves old abandoned items to next-purchase cart
   - Sends "saved for later" notifications

3. **Cleanup Expired Carts** (Weekly on Sunday at 3 AM)
   - Removes carts older than configured threshold

## Running the Application

### Using Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f cart-api

# Stop services
docker-compose down
```

### Local Development

```bash
# Restore packages
dotnet restore

# Run the API
dotnet run --project CartService.API

# Run with watch (auto-reload)
dotnet watch run --project CartService.API
```

### Accessing Services

- **Cart API**: https://localhost:5001
- **Swagger UI**: https://localhost:5001/swagger
- **Hangfire Dashboard**: https://localhost:5001/hangfire
- **Redis Commander**: http://localhost:8081
- **pgAdmin**: http://localhost:8080 (admin@example.com / admin)
- **RabbitMQ Management**: http://localhost:15672 (guest / guest)

## Testing

### Manual Testing with Swagger

1. Navigate to https://localhost:5001/swagger
2. Test the dual-cart workflow:
   - Add items to active cart
   - Move items to next-purchase cart
   - Clear active cart
   - Add new item (triggers auto-activation)

### Sample Test Scenarios

#### Scenario 1: Next-Purchase Auto-Activation

1. Add item to next-purchase cart
2. Clear active cart (or start with empty)
3. Add any item to active cart
4. Verify next-purchase items are automatically moved to active cart
5. Check response for activation message

#### Scenario 2: Abandonment Recovery

1. Add items to active cart
2. Wait for abandonment threshold (30 minutes default)
3. Check Hangfire dashboard for scheduled jobs
4. Verify notification sending (check logs)

## Integration with Other Services

### Inventory Service (gRPC)

```protobuf
service InventoryService {
  rpc CheckStock(StockRequest) returns (StockResponse);
  rpc GetPrice(PriceRequest) returns (PriceResponse);
}
```

### Catalog Service (gRPC)

```protobuf
service CatalogService {
  rpc GetProduct(ProductRequest) returns (ProductResponse);
}
```

### Event Publishing

The service publishes events via RabbitMQ:

- `ItemAddedToCartEvent`
- `ItemRemovedFromCartEvent`
- `ItemMovedBetweenCartsEvent`
- `CartAbandonedEvent`
- `NextPurchaseActivatedEvent`
- `CartMergedEvent`

## Monitoring and Observability

- **Structured Logging**: Serilog with JSON formatting
- **Health Checks**: Available at `/health`
- **Metrics**: Hangfire dashboard for job monitoring
- **Distributed Tracing**: Ready for OpenTelemetry integration

## Security Considerations

- Input validation with FluentValidation
- Rate limiting (can be added)
- Authentication/Authorization (JWT ready)
- Non-root Docker container
- Secure defaults in configuration

## Performance Features

- Redis for high-speed cart operations
- Atomic cart updates
- Efficient background job processing
- gRPC for fast service communication
- Configurable real-time validations

## Future Enhancements

- Real-time notifications with SignalR
- A/B testing framework for cart features
- Machine learning for next-purchase recommendations
- Advanced analytics and reporting
- Multi-currency support
- Internationalization (i18n)

## Additional Documentation

### Complete Persian Documentation
For comprehensive documentation in Persian (Farsi), see:
- **[README_FA.md](./README_FA.md)** - Complete Persian documentation
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Detailed system architecture
- **[DEVELOPMENT.md](./DEVELOPMENT.md)** - Development setup guide
- **[CHECKLIST.md](./CHECKLIST.md)** - Final compliance checklist

### Makefile Commands

This project includes a comprehensive Makefile for easy development and deployment:

```bash
# Quick start for development
make quick-start

# Show all available commands
make help

# Build and test
make build
make test

# Docker operations
make compose-up        # Start all services
make compose-down      # Stop all services
make compose-logs      # View logs

# Development
make run              # Run API locally
make run-watch        # Run with hot reload
make dev-setup        # Setup development environment

# Database operations
make redis-start      # Start Redis container
make postgres-start   # Start PostgreSQL container
make rabbitmq-start   # Start RabbitMQ container

# Health checks
make health-all       # Check all services
make status          # Show services status

# Testing
make test-unit        # Unit tests only
make test-integration # Integration tests
make test-coverage    # Generate coverage report

# Code quality
make lint            # Code analysis
make format          # Format code
make security-scan   # Security scan

# Utilities
make sample-data     # Load sample data
make monitor         # Show monitoring info
make backup          # Backup data
make reset           # Reset everything
```

### Persian Documentation Summary (خلاصه مستندات فارسی)

این پروژه شامل مستندات کاملی به زبان فارسی است که شامل موارد زیر می‌باشد:

- **راهنمای کامل نصب و راه‌اندازی**
- **معماری سیستم با نمودارهای تفصیلی**
- **توضیح ویژگی‌های منحصر به فرد سیستم سبد دوگانه**
- **راهنمای توسعه و تست**
- **نحوه استفاده از API ها**
- **تنظیمات و پیکربندی سیستم**
- **راهنمای عیب‌یابی**

برای مطالعه کامل به فایل `README_FA.md` مراجعه کنید.
