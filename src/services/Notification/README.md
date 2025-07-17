# Notification Microservice with CQRS and Clean Architecture

This is a centralized Notification microservice built with .NET 8, implementing Clean Architecture and CQRS patterns using MediatR.

## 🏗️ Architecture Overview

The service follows Clean Architecture principles with the following layers:

### Domain Layer (`NotificationService.Domain`)
- **Entities**: `NotificationTemplate`, `NotificationLog`
- **Value Objects**: `EmailAddress`, `PhoneNumber`
- **Enums**: `NotificationType`, `NotificationStatus`, `NotificationPriority`
- **Interfaces**: Repository and provider abstractions

### Application Layer (`NotificationService.Application`)
- **Commands**: 
  - `SendEmailCommand` - Send individual emails
  - `SendSmsCommand` - Send individual SMS
  - `QueueNotificationFromEventCommand` - Process events and determine notification type
  - `CreateTemplateCommand` / `UpdateTemplateCommand` - Template management
- **Queries**:
  - `GetNotificationHistoryQuery` - User notification history
  - `GetTemplateQuery` - Template retrieval
  - `GetAllTemplatesQuery` - Template listing
- **Handlers**: Command and Query handlers implementing business logic
- **Behaviors**: MediatR pipeline behaviors for cross-cutting concerns
- **Validators**: FluentValidation validators for all commands

### Infrastructure Layer (`NotificationService.Infrastructure`)
- **Repositories**: MongoDB implementations
- **Providers**: SendGrid (Email), Kavenegar (SMS)
- **Consumers**: MassTransit event consumers
- **Services**: Scriban template engine

### API Layer (`NotificationService.API`)
- **Controllers**: REST API endpoints
- **Configuration**: Service registration and dependency injection

## 🔄 CQRS Workflow

### Primary Workflow
1. **Event occurs** (e.g., user registration, order placement)
2. **MassTransit consumer** receives the event
3. **Consumer creates** `QueueNotificationFromEventCommand`
4. **Command sent** via MediatR
5. **Handler processes** the command:
   - Finds appropriate template
   - Processes template with event data
   - Creates specific command (`SendEmailCommand` or `SendSmsCommand`)
   - Sends command via MediatR
6. **Specific handler** sends notification via provider
7. **Log created** for audit trail

### Direct Notification Workflow
You can also send notifications directly:
```csharp
await mediator.Send(new SendEmailCommand
{
    To = "user@example.com",
    Subject = "Welcome!",
    Body = "Welcome to our platform!",
    UserId = "user123"
});
```

## 🛡️ Resilience and Cross-Cutting Concerns

### MediatR Pipeline Behaviors
1. **LoggingBehavior**: Logs all requests and responses with timing
2. **ValidationBehavior**: FluentValidation integration
3. **ResilienceBehavior**: Polly retry policies for commands

### Retry Policy
- **3 retry attempts** with exponential backoff
- **Only applies to Commands** (not Queries)
- **Configurable** per command type

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- MongoDB
- RabbitMQ

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "NotificationService"
  },
  "SendGrid": {
    "ApiKey": "your-sendgrid-api-key",
    "FromEmail": "noreply@yourcompany.com",
    "FromName": "Your Company"
  },
  "Kavenegar": {
    "ApiKey": "your-kavenegar-api-key",
    "SenderNumber": "your-sender-number"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Running the Service

#### Quick Start with Makefile
```bash
# Complete development setup
make quick-start

# Or step by step:
make dev-setup     # Start dependencies, install packages, build
make run          # Start the API
make seed-templates # Seed sample templates
```

#### Manual Setup
```bash
# Start dependencies
make dev-deps-start

# Install and build
make install
make build

# Run the service
make run
```

#### Using Docker Compose
```bash
# Copy environment variables
cp .env.example .env
# Edit .env with your values

# Start everything
docker-compose up -d

# Check logs
docker-compose logs -f notification-api
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001` 
- Swagger: `https://localhost:5001/swagger`

#### Available Makefile Commands
```bash
make help                 # Show all available commands
make build               # Build the solution
make test                # Run tests
make run                 # Run API locally
make watch               # Run in watch mode
make dev-deps-start      # Start MongoDB & RabbitMQ
make dev-deps-stop       # Stop dependencies
make seed-templates      # Seed notification templates
make docker-build        # Build Docker image
make health-check        # Check service health
make clean               # Clean build artifacts
```

### Seeding Templates

```bash
POST /api/seed/templates
```

This will create sample templates for common scenarios.

## 📧 Template System

### Template Engine
Uses **Scriban** for template processing with Liquid-like syntax:

```html
<h1>Welcome {{ firstName }} {{ lastName }}!</h1>
<p>Registration date: {{ registeredAt | date.to_string '%Y-%m-%d' }}</p>
```

### Template Parameters
Templates support dynamic parameters defined in the template configuration:

```csharp
Parameters = new Dictionary<string, string>
{
    { "firstName", "User's first name" },
    { "lastName", "User's last name" },
    { "registeredAt", "Registration date" }
}
```

## 🔌 Provider Strategy Pattern

### Email Providers
- **SendGrid**: Primary email provider
- **Extensible**: Implement `IEmailProvider` for additional providers

### SMS Providers
- **Kavenegar**: Iranian SMS service
- **Extensible**: Implement `ISmsProvider` for additional providers

### Provider Health Checks
Each provider implements health check functionality for monitoring.

## 📊 Event Integration

### Supported Events
- `IUserRegisteredEvent`
- `IOrderPlacedEvent`
- `IPasswordResetRequestedEvent`

### Adding New Events
1. Create event interface
2. Create MassTransit consumer
3. Register consumer in `ServiceCollectionExtensions`
4. Create corresponding template

### Example Consumer
```csharp
public class UserRegisteredConsumer : IConsumer<IUserRegisteredEvent>
{
    private readonly IMediator _mediator;

    public async Task Consume(ConsumeContext<IUserRegisteredEvent> context)
    {
        var command = new QueueNotificationFromEventCommand
        {
            EventType = "user_registered",
            UserId = context.Message.UserId.ToString(),
            Payload = new Dictionary<string, object>
            {
                { "email", context.Message.Email },
                { "firstName", context.Message.FirstName }
            }
        };
        
        await _mediator.Send(command);
    }
}
```

## 🧪 Testing

### Unit Tests
Test individual handlers using in-memory repositories:

```csharp
[Test]
public async Task SendEmailCommandHandler_Should_SendEmail_Successfully()
{
    // Arrange
    var mockProvider = new Mock<IEmailProvider>();
    var mockRepository = new Mock<INotificationLogRepository>();
    var handler = new SendEmailCommandHandler(mockProvider.Object, mockRepository.Object);
    
    // Act & Assert
    var result = await handler.Handle(command, CancellationToken.None);
    Assert.IsTrue(result);
}
```

### Integration Tests
Test the full pipeline with TestContainers:

```csharp
[Test]
public async Task Should_Process_UserRegistered_Event_EndToEnd()
{
    // Test the complete flow from event to notification
}
```

## 📈 Monitoring and Observability

### Logging
- **Structured logging** with Serilog
- **Request/Response logging** via MediatR behavior
- **Performance metrics** with timing

### Health Checks
- Database connectivity
- Provider health
- RabbitMQ connectivity

### Metrics
- Notification success/failure rates
- Provider performance
- Template usage statistics

## 🔧 Extensibility

### Adding New Notification Types
1. Add to `NotificationType` enum
2. Create provider interface and implementation
3. Update `QueueNotificationFromEventCommandHandler`
4. Create specific command and handler

### Adding New Providers
1. Implement `IEmailProvider` or `ISmsProvider`
2. Register in DI container
3. Configure settings

### Custom Behaviors
Add custom MediatR behaviors for additional cross-cutting concerns:

```csharp
public class CustomBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Custom logic before
        var response = await next();
        // Custom logic after
        return response;
    }
}
```

## 🚀 Deployment

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
```

### Kubernetes
- Deploy with appropriate resource limits
- Configure health check endpoints
- Set up horizontal pod autoscaling

## 📋 API Endpoints

### Notifications
- `POST /api/notifications/email` - Send email directly
- `POST /api/notifications/sms` - Send SMS directly
- `POST /api/notifications/queue-from-event` - Process event
- `GET /api/notifications/history/{userId}` - Get user notification history
- `GET /api/notifications/log/{id}` - Get specific notification log

### Templates
- `GET /api/templates` - List all templates
- `GET /api/templates/{id}` - Get template by ID
- `GET /api/templates/by-name/{name}` - Get template by name
- `POST /api/templates` - Create new template
- `PUT /api/templates/{id}` - Update template

### Admin
- `POST /api/seed/templates` - Seed sample templates

## 🎯 Best Practices

1. **Always validate input** using FluentValidation
2. **Log all operations** for audit trail
3. **Use templates** for consistent messaging
4. **Handle failures gracefully** with retry policies
5. **Monitor provider health** and switch if needed
6. **Test the complete pipeline** including events
7. **Keep templates versioned** for rollback capabilities
8. **Use structured logging** for better observability
