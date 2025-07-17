MicroService Architecture Project
A comprehensive .NET 8 microservices-based e-commerce system with Event-Driven Architecture and Saga Pattern implementation.

📋 Table of Contents
Overview
System Architecture
Technologies Used
Project Structure
Prerequisites
Installation & Setup
Microservices
Building Blocks
Saga Pattern Implementation
API Gateway
Authentication & Authorization
API Documentation
Monitoring & Observability
Testing
Development Guidelines
Contributing
🎯 Overview
This project is a comprehensive e-commerce microservices system built with modern software architecture patterns including Domain-Driven Design (DDD), Command Query Responsibility Segregation (CQRS), Event Sourcing, and Saga Pattern. The system manages products, shipping, authentication, and order processing through distributed transactions.

Key Features
🏗️ Microservices Architecture with complete service isolation
📨 Event-Driven Architecture using RabbitMQ and MassTransit
🔄 Saga Pattern for distributed transaction management
🚪 API Gateway with YARP (Yet Another Reverse Proxy)
🔐 Centralized Authentication with IdentityServer
🐘 PostgreSQL as primary database
🐳 Docker and Docker Compose for containerization
📊 CQRS and MediatR for command and query separation
🔍 Service Discovery and Health Checks
📈 Distributed Logging with Serilog
🏛️ System Architecture
Architecture Patterns
Domain-Driven Design (DDD): Clear domain modeling with aggregates, entities, and value objects
CQRS (Command Query Responsibility Segregation): Separate read and write operations
Event Sourcing: Store events as the source of truth
Saga Pattern: Manage distributed transactions across microservices
Repository Pattern: Data access abstraction
Unit of Work: Transaction management
🔧 Technologies Used
Core Framework
.NET 8 - Primary development framework
ASP.NET Core 8 - Web API framework
C# 12 - Programming language
Data & Persistence
Entity Framework Core 8 - Object-Relational Mapping (ORM)
PostgreSQL 16 - Primary database
Npgsql - PostgreSQL data provider
Messaging & Communication
MassTransit 8 - Distributed application framework
RabbitMQ - Message broker
YARP (Yet Another Reverse Proxy) - API Gateway
gRPC - High-performance RPC framework
Architecture Patterns
MediatR - CQRS and Mediator pattern implementation
AutoMapper - Object-to-object mapping
FluentValidation - Input validation
Authentication & Security
IdentityServer4 - OpenID Connect and OAuth 2.0 framework
JWT Bearer Authentication - Token-based authentication
ASP.NET Core Identity - User management
Infrastructure & DevOps
Docker - Containerization platform
Docker Compose - Multi-container application orchestration
Kubernetes (Ready) - Container orchestration
Monitoring & Logging
Serilog - Structured logging
Health Checks - Application health monitoring
Prometheus (Ready) - Metrics collection
Grafana (Ready) - Metrics visualization
Testing
xUnit - Unit testing framework
Moq - Mocking framework
FluentAssertions - Assertion library
TestContainers - Integration testing with containers
Development Tools
Swagger/OpenAPI - API documentation
Sieve - Filtering, sorting, and pagination
Bogus - Test data generation
📁 Project Structure
📋 Prerequisites
Development Environment
.NET 8 SDK
Docker Desktop
Visual Studio 2022 or VS Code
Optional Tools
Postman - API testing
pgAdmin - PostgreSQL management
RabbitMQ Management - Message queue monitoring
🚀 Installation & Setup
1. Clone the Repository
2. Environment Setup
3. Build and Run with Docker Compose
4. Development Setup (Without Docker)
5. Service Endpoints
Service	URL	Description
API Gateway	http://localhost:8080	Main entry point
Product API	http://localhost:8080/api/products	Product management
Shipping API	http://localhost:8080/api/shipping	Shipping management
Auth Service	http://localhost:5000	Authentication
Swagger UI	http://localhost:8080/swagger	API documentation
RabbitMQ Management	http://localhost:15672	Message queue management
6. Default Credentials
PostgreSQL:

Host: localhost:5432
Database: microservices
Username: postgres
Password: 123
RabbitMQ:

Management UI: http://localhost:15672
Username: guest
Password: guest
Default Admin User:

Email: admin@microservice.com
Password: Admin123!
🎯 Microservices
Product Service
Manages products, categories, brands, and product-related features.

Features:

✅ Product CRUD operations
✅ Category management
✅ Brand management
✅ Product Q&A system
✅ Product image management
✅ Product reviews and ratings
✅ Product variants and specifications
✅ Inventory management
Key Entities:

Product
Category
Brand
Question/Answer
Review
ProductImage
ProductVariant
API Endpoints:

Shipping Service
Manages shipping methods, cost calculation, and shipment tracking.

Features:

✅ Shipping methods management
✅ Shipping cost calculation
✅ Shipment tracking
✅ Carrier management
✅ Multi-role support (Customer, Vendor, Admin)
✅ Real-time tracking updates
✅ Shipping zones and rules
Key Entities:

ShippingMethod
Shipment
Carrier
TrackingEvent
ShippingZone
API Endpoints:

Auth Service
Centralized authentication and authorization with multi-role support.

Features:

✅ JWT-based authentication
✅ Role-based authorization (Admin, Customer, Vendor)
✅ User management
✅ Refresh token handling
✅ Password policies
✅ Account lockout
✅ Two-factor authentication ready
Roles:

Admin: Full system access
Customer: Product browsing, ordering
Vendor: Product management, order fulfillment
API Endpoints:

🧱 Building Blocks
Event-Driven Messaging
CQRS Pattern Implementation
🔄 Saga Pattern Implementation
The system uses Saga Pattern for managing complex distributed transactions using MassTransit State Machine.

Order Processing Saga
Saga Workflow:
📝 Order Created → Reserve inventory
📦 Inventory Reserved → Process payment
💳 Payment Processed → Create shipment
🚚 Shipment Created → Order completed
✅ Order Completed → Notify customer
Compensation Actions:
❌ Payment Failed → Release inventory
❌ Inventory Unavailable → Cancel order
❌ Shipment Failed → Refund payment
🚪 API Gateway
YARP (Yet Another Reverse Proxy) configuration for:

Features:
Request Routing to appropriate microservices
Authentication & Authorization enforcement
Rate Limiting and throttling
Correlation ID injection for request tracing
Load Balancing across service instances
Circuit Breaker pattern implementation
Request/Response transformation
Configuration Example:
Custom Middleware:
🔐 Authentication & Authorization
Identity Server Configuration