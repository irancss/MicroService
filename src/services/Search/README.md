# Search Service

A high-performance, personalized search microservice built with .NET 8, Elasticsearch, and Clean Architecture principles.

## Features

- **Advanced Full-Text Search**: Powerful, linguistic-aware search across product names, descriptions, tags, and categories
- **Spell Correction**: Automatic typo detection and "Did You Mean?" suggestions
- **Auto-Complete**: Real-time search suggestions as users type
- **Dynamic Faceted Search**: Flexible filtering by category, brand, price, rating, and custom attributes
- **Personalized Results**: AI-powered result boosting based on user behavior and preferences
- **High Performance**: Sub-200ms response times with Elasticsearch optimization
- **Scalable Architecture**: Load-balanced with NGINX, horizontally scalable API instances
- **Real-time Sync**: Automatic index updates via RabbitMQ event consumption

## Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│  Controllers, Middleware, Health Checks, Swagger           │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│     CQRS Handlers, Commands, Queries, Behaviors            │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│    Entities, Value Objects, Interfaces, Business Logic     │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  Elasticsearch, RabbitMQ, External Services, Repositories  │
└─────────────────────────────────────────────────────────────┘
```

### Technology Stack

- **.NET 8**: High-performance runtime with minimal APIs
- **MediatR**: CQRS implementation with pipeline behaviors
- **FluentValidation**: Robust input validation
- **Elasticsearch 8.12**: Advanced search engine with ML capabilities
- **RabbitMQ**: Reliable message queuing for event-driven updates
- **MassTransit**: Service bus abstraction for messaging
- **Serilog**: Structured logging with multiple sinks
- **NGINX**: Load balancing, caching, and rate limiting
- **Docker**: Containerized deployment

## API Endpoints

### Search Products
```http
GET /api/search/products?query=laptop&category=electronics&sortBy=Relevance&page=1&size=20
```

**Query Parameters:**
- `query`: Search text (optional)
- `userId`: User ID for personalization (optional)
- `category`: Category filter
- `brand`: Brand filter
- `minPrice`, `maxPrice`: Price range filters
- `minRating`, `maxRating`: Rating range filters
- `isAvailable`: Availability filter
- `tags`: Comma-separated tags
- `attr_{name}`: Dynamic attribute filters (e.g., `attr_color=red,blue`)
- `sortBy`: Sorting option (Relevance, PriceLowToHigh, NewestArrivals, etc.)
- `page`, `size`: Pagination
- `includeFacets`: Include facet aggregations
- `includeSuggestions`: Include spell correction suggestions
- `enablePersonalization`: Enable user-based result boosting

### Auto-Complete Suggestions
```http
GET /api/search/suggestions?query=lap&maxSuggestions=10
```

### Index Management
```http
POST /api/index/products
DELETE /api/index/products/{productId}
POST /api/index/products/bulk
```

## Personalization Features

The search service provides intelligent result boosting based on:

- **Preferred Categories**: Categories the user frequently views/purchases
- **Preferred Brands**: Brands the user shows affinity for
- **Price Range Preferences**: User's typical spending patterns
- **Attribute Preferences**: Preferred product characteristics (color, size, etc.)
- **Recent Interactions**: Recently viewed products for context

## Performance Optimizations

### Elasticsearch Configuration
- **Custom Analyzers**: Optimized text analysis for product search
- **Smart Mapping**: Efficient field types and indexing strategies
- **Completion Suggesters**: Fast auto-complete with context awareness
- **Aggregation Caching**: Facet result caching for improved performance

### Caching Strategy
- **NGINX Caching**: Response caching at the reverse proxy level
- **Search Result Caching**: 1-minute cache for search results
- **Suggestion Caching**: 30-second cache for auto-complete
- **Facet Caching**: Longer cache for facet aggregations

### Rate Limiting
- **Search Queries**: 10 requests/second with burst capacity
- **Auto-Complete**: 20 requests/second for responsive UX
- **Admin Operations**: 5 requests/second for index management

## Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for development)
- At least 4GB RAM available for Elasticsearch

### Running with Docker Compose

1. **Clone and navigate to the search service directory**
```bash
cd src/services/Search
```

2. **Start the entire stack**
```bash
docker-compose up -d
```

3. **Wait for services to be healthy**
```bash
docker-compose ps
```

4. **Create the Elasticsearch index**
```bash
curl -X PUT "localhost:9200/products" -H "Content-Type: application/json" -d @elasticsearch-mapping.json
```

5. **Test the API**
```bash
# Health check
curl http://localhost/health

# Sample search
curl "http://localhost/api/search/products?query=laptop&sortBy=Relevance"

# Auto-complete
curl "http://localhost/api/search/suggestions?query=lap"
```

### Access Points
- **Search API**: http://localhost (via NGINX)
- **API Documentation**: http://localhost/swagger
- **Elasticsearch**: http://localhost:9200
- **Kibana**: http://localhost:5601
- **RabbitMQ Management**: http://localhost:15672 (admin/admin123)
- **Grafana Monitoring**: http://localhost:3000 (admin/admin123)

## Event-Driven Updates

The search service automatically stays synchronized with product data through RabbitMQ events:

### Consumed Events
- `ProductCreatedEvent`: Adds new products to the search index
- `ProductUpdatedEvent`: Updates existing product information
- `ProductDeletedEvent`: Removes products from the index

### Event Format
```json
{
  "productId": "12345",
  "name": "Gaming Laptop",
  "description": "High-performance gaming laptop...",
  "category": "Electronics",
  "brand": "TechBrand",
  "price": 1299.99,
  "averageRating": 4.5,
  "isAvailable": true,
  "attributes": {
    "color": "Black",
    "screenSize": "15.6 inch",
    "processor": "Intel i7"
  }
}
```

## Monitoring & Health Checks

### Health Check Endpoints
- `/health`: Overall service health
- `/health/ready`: Readiness probe for Kubernetes
- `/health/live`: Liveness probe for Kubernetes

### Metrics & Monitoring
- **Prometheus**: Metrics collection at `:9090`
- **Grafana**: Dashboards and alerting at `:3000`
- **Application Insights**: Distributed tracing (configurable)
- **Structured Logging**: JSON logs with correlation IDs

### Key Metrics Tracked
- Search response times
- Cache hit rates
- Elasticsearch cluster health
- RabbitMQ message processing
- Error rates and exceptions
- Request volumes and patterns

## Development

### Local Development Setup

1. **Install dependencies**
```bash
dotnet restore
```

2. **Start infrastructure services**
```bash
docker-compose up elasticsearch rabbitmq -d
```

3. **Run the API**
```bash
cd SearchService.API
dotnet run
```

4. **Access Swagger UI**
```
https://localhost:7090/swagger
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__Elasticsearch` | Elasticsearch connection string | `http://localhost:9200` |
| `ConnectionStrings__RabbitMQ` | RabbitMQ connection string | `amqp://guest:guest@localhost:5672` |
| `UserPersonalizationService__BaseUrl` | User personalization service URL | `https://localhost:7001` |
| `Elasticsearch__Username` | Elasticsearch username | - |
| `Elasticsearch__Password` | Elasticsearch password | - |

### Custom Configuration

The service supports extensive configuration through `appsettings.json`:

- Elasticsearch settings (timeouts, retry policies)
- RabbitMQ configuration (queue names, retry policies)
- Caching settings (TTL, sizes)
- Logging levels and sinks
- Rate limiting rules
- CORS policies

## Production Deployment

### Kubernetes Deployment

See the `k8s/` directory for Kubernetes manifests including:
- Deployment configurations
- Service definitions
- ConfigMaps and Secrets
- Horizontal Pod Autoscaler
- Ingress controllers

### Security Considerations

- **Authentication**: Integrate with your identity provider
- **Authorization**: Implement role-based access control
- **HTTPS**: Enable TLS termination at load balancer
- **Secrets Management**: Use Kubernetes secrets or Azure Key Vault
- **Network Policies**: Restrict inter-service communication
- **Input Validation**: All inputs are validated with FluentValidation

### Scaling Guidelines

- **API Instances**: Scale based on CPU/memory usage
- **Elasticsearch**: Scale horizontally with proper shard strategy
- **RabbitMQ**: Use clustering for high availability
- **NGINX**: Scale for geographic distribution

## Contributing

1. Follow Clean Architecture principles
2. Write comprehensive tests
3. Use conventional commit messages
4. Update documentation for new features
5. Ensure proper error handling and logging

## License

This project is licensed under the MIT License - see the LICENSE file for details.
