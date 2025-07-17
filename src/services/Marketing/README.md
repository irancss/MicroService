# Marketing Microservice - Clean Architecture with CQRS

This Marketing microservice implements Clean Architecture with CQRS using MediatR, designed to manage marketing campaigns, landing pages, and user segmentation.

## Quick Start / شروع سریع

```bash
# Complete setup and run / راه‌اندازی کامل و اجرا
make quick-start

# Or step by step / یا قدم به قدم
make setup
make build
make db-update
make run
```

## Available Commands / دستورات موجود

Run `make help` to see all available commands:

```bash
make help
```

### Development Commands / دستورات توسعه
- `make setup` - Setup development environment / راه‌اندازی محیط توسعه
- `make clean` - Clean build artifacts / پاک‌سازی فایل‌های ساخت
- `make build` - Build the solution / ساخت راه‌حل
- `make test` - Run tests / اجرای تست‌ها
- `make run` - Run the API locally / اجرای API به صورت محلی
- `make watch` - Run with hot reload / اجرا با بارگذاری مجدد خودکار

### Database Commands / دستورات دیتابیس
- `make migration-add MIGRATION_NAME=YourMigrationName` - Add new migration
- `make db-update` - Update database / بروزرسانی دیتابیس
- `make db-reset` - Reset database / ریست دیتابیس

### Docker Commands / دستورات داکر
- `make docker-build` - Build Docker image / ساخت image داکر
- `make docker-compose-up` - Start with docker-compose / شروع با docker-compose
- `make docker-compose-down` - Stop docker-compose services / توقف سرویس‌ها

## Architecture Overview

The service follows Clean Architecture principles with clear separation of concerns:

- **Domain Layer**: Contains entities, value objects, enums, and repository interfaces
- **Application Layer**: Contains CQRS commands/queries, handlers, DTOs, and business logic
- **Infrastructure Layer**: Contains database context, repositories, background jobs, and external integrations
- **API Layer**: Contains REST API controllers and dependency injection setup

## Features Implemented

### CQRS Commands
1. **CreateCampaignCommand**: Create new marketing campaigns with validation
2. **UpdateLandingPageCommand**: Update landing page content and metadata
3. **AssignUserToSegmentCommand**: Assign users to segments (triggered by background jobs)

### CQRS Queries
1. **GetCampaignBySlugQuery**: Retrieve campaign data for dynamic landing pages
2. **GetUserPersonalizationQuery**: Get user segments and relevant campaigns
3. **GetAllCampaignsQuery**: Admin dashboard campaign listing

### Domain Entities
- **Campaign**: Marketing campaigns with budgets, metrics, and target segments
- **LandingPage**: Dynamic content pages with SEO metadata
- **UserSegment**: User grouping based on criteria
- **UserSegmentMembership**: User-segment relationships with expiry

### Value Objects
- **Money**: Amount and currency with operations
- **DateRange**: Campaign date management
- **SegmentCriteria**: Flexible user segmentation rules
- **CampaignMetrics**: Performance tracking (CTR, ROAS, etc.)

## Technology Stack

- **.NET 8**: Core framework
- **MediatR**: CQRS pattern implementation
- **FluentValidation**: Command validation
- **Entity Framework Core**: ORM with PostgreSQL
- **Hangfire**: Background job processing
- **MassTransit + RabbitMQ**: Message bus for inter-service communication
- **Swagger**: API documentation

## Database Design

The service uses PostgreSQL with the following key tables:
- `Campaigns`: Campaign details with JSON columns for metrics and target segments
- `LandingPages`: Page content with SEO metadata
- `UserSegments`: Segment definitions with JSON criteria
- `UserSegmentMemberships`: User-segment assignments

## Background Jobs

### UserSegmentationJob
- Processes user data against segment criteria
- Automatically assigns users to appropriate segments
- Supports bulk processing for performance
- Configurable retry policies

## API Endpoints

### Campaigns
- `GET /api/campaigns` - List all campaigns
- `GET /api/campaigns/{slug}` - Get campaign by slug
- `POST /api/campaigns` - Create new campaign

### Landing Pages
- `PUT /api/landingpages/{id}` - Update landing page

### Personalization
- `GET /api/personalization/user/{userId}` - Get user personalization data

## Message Consumers

### UserRegisteredConsumer
Listens for user registration events and automatically assigns new users to default segments.

## Configuration

### Environment Variables
Copy `.env.example` to `.env` and configure:

```bash
cp .env.example .env
# Edit .env with your settings
```

### Connection Strings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MarketingDB;Username=postgres;Password=password",
    "RabbitMQ": "amqp://localhost"
  }
}
```

### Required Services
- PostgreSQL database
- RabbitMQ message broker
- Redis (optional, for caching)
- Hangfire dashboard (available at `/hangfire`)

## Docker Deployment

### Using Docker Compose
```bash
# Start all services
make docker-compose-up

# Or with specific profiles
docker-compose --profile tools up -d    # Include pgAdmin
docker-compose --profile monitoring up -d # Include Prometheus & Grafana
```

### Services Available
- **Marketing API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Hangfire Dashboard**: http://localhost:5000/hangfire
- **pgAdmin**: http://localhost:8090 (with tools profile)
- **RabbitMQ Management**: http://localhost:15672
- **Grafana**: http://localhost:3000 (with monitoring profile)

## Development Setup

1. **Prerequisites**:
   ```bash
   # Install required tools
   make dev-tools
   ```

2. **Database Setup**:
   ```bash
   # Using Docker
   docker-compose up postgres -d
   
   # Or install PostgreSQL locally
   # Then run:
   make db-update
   ```

3. **Message Broker Setup**:
   ```bash
   # Using Docker
   docker-compose up rabbitmq -d
   
   # Or install RabbitMQ locally
   ```

4. **Run the Service**:
   ```bash
   make run
   # or with hot reload
   make watch
   ```

## Monitoring and Health Checks

### Health Endpoints
- `GET /health` - Overall health status
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

### Monitoring
```bash
# Check service health
make health-check

# View logs
make logs

# Open monitoring dashboards
make swagger    # API documentation
make hangfire   # Background jobs
```

## Testing Strategy

The architecture supports comprehensive testing:
- Unit tests for domain logic and value objects
- Integration tests for repositories and database operations
- Handler tests for CQRS command/query logic
- API tests for endpoint behavior

## Validation & Error Handling

- FluentValidation for command validation
- Automatic validation pipeline using MediatR behaviors
- Structured error responses for API endpoints
- Comprehensive logging throughout the application

## Scalability Considerations

- Async/await throughout for better performance
- Background job processing for long-running operations
- Message-based communication for loose coupling
- Repository pattern for testability and flexibility
- Clean separation enables independent scaling of layers

## Production Deployment

```bash
# Build for production
make publish

# Build Docker image for production
make publish-docker

# Deploy with docker-compose
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Troubleshooting

### Common Issues

1. **Database Connection Issues**:
   ```bash
   # Check PostgreSQL is running
   docker-compose ps postgres
   
   # Reset database
   make db-reset
   ```

2. **Package Restore Issues**:
   ```bash
   # Clean and restore
   make clean
   make restore
   ```

3. **Port Conflicts**:
   ```bash
   # Check what's using the ports
   netstat -tulpn | grep :5000
   
   # Kill the process or change ports in appsettings.json
   ```

This implementation provides a solid foundation for a scalable marketing microservice with clear separation of concerns, comprehensive validation, and robust error handling.
