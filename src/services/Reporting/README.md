# Reporting Service
یک میکروسرویس پیشرفته برای پردازش و تحلیل داده‌های فروش با معماری CQRS

## ویژگی‌ها
- **معماری CQRS**: پیاده‌سازی کامل Command Query Responsibility Segregation با MediatR
- **Clean Architecture**: جداسازی واضح Domain/Application/Infrastructure/API
- **Star Schema**: طراحی دیتابیس بر اساس warehouse برای تحلیل‌های سریع
- **Event-Driven**: پردازش event ها برای ETL خودکار با MassTransit
- **Background Jobs**: جمع‌آوری خودکار داده‌ها با Hangfire
- **Docker Ready**: آماده برای deployment

## معماری

### Domain Layer
- `OrderFact`: جدول اصلی فروش (Fact Table)
- `DailySalesAggregate`: تجمیع روزانه فروش
- Dimension Tables: `ProductDimension`, `CustomerDimension`, `DateDimension`

### Application Layer
#### Commands (CQRS)
- `ProcessOrderCompletedEventCommand`: پردازش order های تکمیل شده
- `AggregateDailySalesCommand`: تجمیع فروش روزانه
- `CreateProductDimensionCommand`: ایجاد dimension محصول
- `UpdateCustomerDimensionCommand`: بروزرسانی dimension مشتری
- `BulkInsertOrderFactsCommand`: درج bulk داده‌های فروش

#### Queries (CQRS)
- `GetDailySalesReportQuery`: گزارش فروش روزانه
- `GetTopSellingProductsQuery`: محصولات پرفروش
- `GetCustomerAnalyticsQuery`: تحلیل مشتریان
- `GetSalesComparisonQuery`: مقایسه فروش دوره‌ای
- `GetProductPerformanceQuery`: عملکرد محصولات

### Infrastructure Layer
- **Repository Pattern**: دسترسی بهینه به داده‌ها
- **Event Consumers**: پردازش event های سیستم با MassTransit
- **Background Services**: کارهای زمان‌بندی شده با Hangfire

### API Layer
- **Sales Reports Controller**: گزارش‌های فروش
- **Product Analytics Controller**: تحلیل محصولات
- **Data Management Controller**: مدیریت داده‌ها
- **Test Controller**: تست عملکرد سیستم

## شروع سریع

### نیازمندی‌ها
- .NET 8 SDK
- Docker & Docker Compose
- Make (اختیاری)

### راه‌اندازی
```bash
# راه‌اندازی سریع با Docker
make quick-start

# یا به صورت دستی
docker-compose up -d
dotnet run --project ReportingService.API
```

## دستورات Make

### توسعه
```bash
make build          # ساخت پروژه
make test           # اجرای تست‌ها
make run            # اجرای سرویس
make watch          # اجرا در حالت watch
make clean          # پاک کردن فایل‌های build
```

### Docker
```bash
make docker-build   # ساخت تصویر Docker
make docker-run     # اجرای container
make docker-stop    # متوقف کردن container
make docker-logs    # مشاهده logs
```

### دیتابیس
```bash
make db-migrate     # اجرای migration ها
make db-seed        # پر کردن داده‌های اولیه
make db-reset       # بازنشانی دیتابیس
make db-backup      # پشتیبان‌گیری از دیتابیس
```

## API Endpoints

### Sales Reports
- `GET /api/sales-reports/daily` - گزارش فروش روزانه
- `GET /api/sales-reports/comparison` - مقایسه فروش
- `GET /api/sales-reports/trends` - روند فروش

### Product Analytics
- `GET /api/product-analytics/top-selling` - محصولات پرفروش
- `GET /api/product-analytics/performance` - عملکرد محصولات

### Data Management
- `POST /api/data-management/aggregate-daily` - تجمیع روزانه
- `POST /api/data-management/process-events` - پردازش event ها

### Environment Variables

```env
# Database
ConnectionStrings__DefaultConnection=Host=localhost;Database=reporting_db;Username=postgres;Password=password

# RabbitMQ
MessageBroker__Host=localhost
MessageBroker__Username=guest
MessageBroker__Password=guest

# Redis
Redis__ConnectionString=localhost:6379

# Hangfire
Hangfire__Dashboard__Enabled=true
Hangfire__RecurringJobs__Enabled=true
```

## CQRS Commands و Queries تایید شده

### Commands (20 مورد)
```bash
ProcessOrderCompletedEventCommand
AggregateDailySalesCommand  
CreateProductDimensionCommand
UpdateCustomerDimensionCommand
BulkInsertOrderFactsCommand
CreateDateDimensionCommand
UpdateProductDimensionCommand
DeleteOrderFactCommand
CreateCustomerDimensionCommand
SyncDimensionTablesCommand
```

### Queries (10 مورد)
```bash
GetDailySalesReportQuery
GetTopSellingProductsQuery
GetCustomerAnalyticsQuery
GetSalesComparisonQuery
GetProductPerformanceQuery
GetMonthlySalesReportQuery
GetCustomerPurchaseHistoryQuery
GetProductInventoryQuery
GetRevenueTrendsQuery
GetTopCustomersQuery
```

2. **AggregateDailySalesCommand**
   - Pre-calculates daily sales totals for performance optimization
   - Scheduled via Hangfire background jobs
   - Updates aggregated metrics for fast dashboard queries

### Queries (Read Side)
1. **GetDailySalesReportQuery**
   - Retrieves pre-aggregated daily sales data
   - Supports date range filtering and currency selection
   - Returns comprehensive sales metrics and summaries

2. **GetTopSellingProductsQuery**
   - Executes complex analytical queries on data warehouse
   - Supports multiple ranking criteria (Revenue, Quantity, OrderCount)
   - Includes filtering by category, brand, and time period

## 🛠️ Technology Stack

### Core Framework
- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API endpoints
- **Entity Framework Core** - ORM for PostgreSQL

### CQRS & Messaging
- **MediatR 12.2.0** - CQRS implementation and in-process messaging
- **FluentValidation** - Request validation
- **AutoMapper** - Object mapping

### Data & Analytics
- **PostgreSQL** - Analytical database with TimescaleDB capabilities
- **Entity Framework Core** - Data access layer
- **Star Schema Design** - Optimized for analytical queries

### Background Processing
- **Hangfire** - Background job processing and scheduling
- **Hangfire.PostgreSql** - PostgreSQL storage for Hangfire

### Message Broker
- **MassTransit** - Service bus abstraction
- **RabbitMQ** - Message broker for event-driven architecture

### Monitoring & Health
- **Swagger/OpenAPI** - API documentation
- **Health Checks** - Database and service health monitoring
- **Structured Logging** - Comprehensive logging with Serilog

## 📊 Database Schema (Star Schema)

### Fact Tables
- **order_facts** - Central fact table with order transaction data
- **daily_sales_aggregates** - Pre-aggregated daily metrics

### Dimension Tables
- **date_dimensions** - Time dimension for temporal analytics
- **customer_dimensions** - Customer master data
- **product_dimensions** - Product master data

[View detailed schema documentation](docs/database-schema.md)

## 🔄 ETL Process Flow

1. **Extract**: External microservice publishes `OrderCompletedEvent`
2. **Transform**: 
   - MassTransit consumer receives event
   - Triggers `ProcessOrderCompletedEventCommand` via MediatR
   - Validates and transforms data
   - Enriches with dimension lookups
3. **Load**: 
   - Inserts/updates dimension tables
   - Creates fact records in star schema
   - Maintains data consistency and referential integrity

## 📈 Background Jobs & Scheduling

### Daily Aggregation (2:00 AM daily)
```csharp
RecurringJob.AddOrUpdate<SalesAggregationJobs>(
    "daily-sales-aggregation",
    job => job.RunDailySalesAggregation(),
    Cron.Daily(2));
```

### Weekly Cleanup (Sunday 3:00 AM)
```csharp
RecurringJob.AddOrUpdate<SalesAggregationJobs>(
    "weekly-cleanup", 
    job => job.CleanupOldData(1095),
    Cron.Weekly(DayOfWeek.Sunday, 3));
```

## 🔧 Configuration

### Database Connections
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ReportingService;Username=reporting_user;Password=reporting_pass;",
    "HangfireConnection": "Host=localhost;Database=ReportingService_Hangfire;Username=reporting_user;Password=reporting_pass;",
    "RabbitMQ": "rabbitmq://localhost"
  }
}
```

### Service Configuration
```json
{
  "ReportingService": {
    "RetentionDays": 1095,
    "DefaultCurrency": "USD",
    "MaxReportDays": 365,
    "MaxTopProductsCount": 100
  }
}
```

## 📚 API Endpoints

### Sales Reports
```http
GET /api/salesreports/daily?fromDate=2024-01-01&toDate=2024-01-31&currency=USD
GET /api/salesreports/daily/last30days?currency=USD
GET /api/salesreports/monthly/2024?currency=USD
```

### Product Analytics
```http
GET /api/productanalytics/top-selling?fromDate=2024-01-01&toDate=2024-01-31&topCount=10&rankBy=Revenue
GET /api/productanalytics/top-selling/last30days?topCount=20&category=Electronics
GET /api/productanalytics/by-category/Electronics?fromDate=2024-01-01&toDate=2024-01-31
GET /api/productanalytics/by-brand/Apple?fromDate=2024-01-01&toDate=2024-01-31
```

### Data Management
```http
POST /api/datamanagement/aggregate/daily/2024-01-15?currency=USD
POST /api/datamanagement/aggregate/historical?fromDate=2024-01-01&toDate=2024-01-31
GET /api/datamanagement/jobs/{jobId}/status
```

### Testing Endpoints (Development)
```http
POST /api/testdata/generate-sample-orders/50
```

## 🏃 Running the Service

### Prerequisites
- .NET 8 SDK
- PostgreSQL 14+
- RabbitMQ
- Docker (optional, for containerized dependencies)

### Setup Steps

1. **Clone and Build**
```bash
git clone <repository-url>
cd src/services/Reporting
dotnet build
```

2. **Database Setup**
```bash
# Create databases
createdb ReportingService
createdb ReportingService_Hangfire

# Run migrations
dotnet ef database update --project ReportingService.Infrastructure --startup-project ReportingService.API
```

3. **Start Dependencies**
```bash
# Start RabbitMQ (Docker)
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# Or install locally
# Start PostgreSQL service
```

4. **Run the Service**
```bash
cd ReportingService.API
dotnet run
```

5. **Access Endpoints**
- **Swagger UI**: http://localhost:5000
- **Hangfire Dashboard**: http://localhost:5000/hangfire
- **Health Checks**: http://localhost:5000/health

## 🧪 Testing

### Generate Sample Data
```bash
# Generate 50 sample orders for testing
curl -X POST "http://localhost:5000/api/testdata/generate-sample-orders/50"

# Trigger daily aggregation
curl -X POST "http://localhost:5000/api/datamanagement/aggregate/daily/2024-01-15"

# Get daily sales report
curl "http://localhost:5000/api/salesreports/daily/last30days"

# Get top selling products
curl "http://localhost:5000/api/productanalytics/top-selling/last30days?topCount=10"
```

## 📊 Performance Considerations

### Indexing Strategy
- **Composite indexes** on fact tables for analytical queries
- **Covering indexes** for frequently accessed columns
- **Partitioning** by date for large datasets

### Caching Strategy
- **Pre-aggregated data** in `daily_sales_aggregates` table
- **Materialized views** for complex recurring queries
- **In-memory caching** for dimension lookups

### Query Optimization
- **Star schema** design for optimal join performance
- **Column-oriented** storage considerations
- **Query result pagination** for large datasets

## 🔍 Monitoring & Observability

### Health Checks
- Database connectivity
- Hangfire server status
- Message broker connectivity

### Logging
- Structured logging with correlation IDs
- Performance metrics for analytical queries
- Background job execution tracking

### Metrics
- Query execution times
- Data ingestion rates
- Business KPIs and trends

## 🔄 Deployment

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "ReportingService.API.dll"]
```

### Kubernetes Deployment
- Horizontal pod autoscaling
- Database connection pooling
- Background job processing separation

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Implement comprehensive unit tests
3. Add integration tests for API endpoints
4. Update documentation for new features
5. Follow CQRS patterns for new commands/queries

## 📄 License

This project is licensed under the MIT License.
