# سرویس تخفیف (Discount Service)

یک میکروسرویس کامل و پیشرفته برای مدیریت و محاسبه انواع تخفیف‌ها در پلتفرم‌های تجارت الکترونیک، پیاده‌سازی شده با .NET 8 و معماری Clean Architecture.

## 🚀 ویژگی‌های کلیدی

### 🎯 انواع تخفیف پشتیبانی شده:
- **تخفیف درصدی (Percentage)**: با قابلیت تعیین حداکثر مبلغ تخفیف
- **تخفیف مبلغ ثابت (Fixed Amount)**: مبلغ مشخص تخفیف
- **خرید X، دریافت Y رایگان (BOGO)**: کمپین‌های پیشرفته خرید و هدیه
- **ارسال رایگان (Free Shipping)**: تخفیف هزینه ارسال

### 🏗️ معماری و فناوری:
- **.NET 8**: آخرین نسخه فریمورک .NET
- **Clean Architecture**: جداسازی کامل لایه‌ها و dependencies
- **PostgreSQL**: پایگاه داده قدرتمند و مقیاس‌پذیر
- **Redis**: کش هوشمند برای عملکرد بالا
- **RabbitMQ + MassTransit**: پیام‌رسانی event-driven
- **Docker**: کانتینریزه شدن کامل

### 🔧 الگوهای طراحی:
- **CQRS با MediatR**: جداسازی Command و Query
- **Repository Pattern**: انتزاع لایه داده
- **Unit of Work**: مدیریت تراکنش‌ها
- **Specification Pattern**: منطق پیچیده کوئری‌ها

### 🛡️ امنیت و کیفیت:
- **JWT Authentication**: احراز هویت امن
- **Role-based Authorization**: کنترل دسترسی مبتنی بر نقش
- **FluentValidation**: اعتبارسنجی پیشرفته
- **Rate Limiting**: محدودیت درخواست
- **Health Checks**: مانیتورینگ سلامت سرویس

## 📁 ساختار پروژه

```
DiscountService/
├── DiscountService.Domain/          # لایه Domain - منطق کسب و کار اصلی
│   ├── Entities/                    # موجودیت‌های اصلی (Discount, DiscountUsageHistory)
│   ├── Enums/                       # انواع تعریف شده
│   ├── ValueObjects/                # Value Objects (Cart, DiscountCalculationResult)
│   ├── Services/                    # سرویس‌های Domain (DiscountCalculationService)
│   └── Events/                      # رویدادهای Domain
├── DiscountService.Application/     # لایه Application - Use Cases
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Features/                    # Commands و Queries (CQRS)
│   ├── Handlers/                    # MediatR handlers
│   ├── Interfaces/                  # رابط‌های Repository و Service
│   ├── Mappings/                    # پروفایل‌های AutoMapper
│   └── Validators/                  # اعتبارسنج‌های FluentValidation
├── DiscountService.Infrastructure/  # لایه Infrastructure - دسترسی به داده
│   ├── Data/                        # Entity Framework DbContext
│   ├── Repositories/                # پیاده‌سازی Repository ها
│   ├── Services/                    # پیاده‌سازی سرویس‌های خارجی
│   └── Messaging/                   # Consumer های RabbitMQ
└── DiscountService.API/             # لایه API - Controllers و ورودی
    ├── Controllers/                 # کنترلرهای API
    └── Program.cs                   # نقطه ورود اپلیکیشن
```

## 🎯 ویژگی‌های کلیدی

### انواع تخفیف:
- **تخفیف درصدی**: اعمال درصد تخفیف (مثل 20% تخفیف)
- **تخفیف مبلغ ثابت**: اعمال مبلغ مشخص تخفیف (مثل 50 هزار تومان تخفیف)
- **خرید X، دریافت Y رایگان**: کمپین‌های ترویجی (مثل خرید 2 عدد، 1 عدد رایگان)
- **ارسال رایگان**: حذف هزینه ارسال با شرایط خاص

### روش‌های اعمال تخفیف:
- **کدهای تخفیف**: کدهای وارد شده توسط کاربر (مثل "RAMADAN20")
- **تخفیف‌های خودکار**: کمپین‌های عمومی که به صورت خودکار اعمال می‌شوند
- **تخفیف‌های اختصاصی**: تخفیف‌های هدفمند برای کاربران خاص

### قوانین کسب و کار و محدودیت‌ها:
- **دوره اعتبار**: تاریخ شروع و پایان برای هر تخفیف
- **محدودیت استفاده**: محدودیت‌های سراسری و برای هر کاربر
- **حداقل مبلغ سبد**: شرایط آستانه مورد نیاز
- **Applicability**: Cart-wide, specific products, or specific categories
- **Combinability**: Control whether discounts can be stacked
- **Priority System**: Intelligent discount selection for maximum savings

### API Endpoints

#### Public Endpoints
- `POST /api/discounts/calculate` - Calculate best discount for a cart

#### Admin Endpoints (Requires Admin Role)
- `POST /api/admin/discounts` - Create new discount
- `GET /api/admin/discounts/{id}` - Get discount by ID
- `GET /api/admin/discounts` - Get paginated discounts list
- `PUT /api/admin/discounts/{id}` - Update existing discount
- `DELETE /api/admin/discounts/{id}` - Deactivate discount
- `GET /api/admin/discounts/{id}/usage-history` - Get usage history

#### User Endpoints (Requires Authentication)
- `GET /api/users/discounts/my-discount-history` - Get user's discount history

## 🛠️ Technology Stack

### Core Technologies
- **.NET 8**: Latest framework with performance improvements
- **PostgreSQL**: Primary database with Entity Framework Core
- **Redis**: High-performance caching layer
- **RabbitMQ**: Message broker for event-driven communication
- **Docker**: Containerization for deployment

### Libraries & Packages
- **MediatR**: CQRS pattern implementation
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **MassTransit**: Message bus abstraction
- **Swashbuckle**: OpenAPI/Swagger documentation

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL (if running locally)
- Redis (if running locally)
- RabbitMQ (if running locally)

### Quick Start with Docker Compose

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd discount-service
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Access the API**
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080/swagger
   - Health Check: http://localhost:8080/health

### Local Development Setup

1. **Update connection strings in `appsettings.Development.json`**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=DiscountServiceDb_Dev;Username=postgres;Password=your_password",
       "Redis": "localhost:6379",
       "RabbitMQ": "rabbitmq://guest:guest@localhost:5672/"
     }
   }
   ```

2. **Run the application**
   ```bash
   cd DiscountService.API
   dotnet run
   ```

## 📊 Database Schema

### Discount Table
```sql
CREATE TABLE Discounts (
    Id UUID PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(1000) NOT NULL,
    CouponCode VARCHAR(50) UNIQUE,
    Type INT NOT NULL,
    Value DECIMAL(18,2) NOT NULL,
    StartDate TIMESTAMP NOT NULL,
    EndDate TIMESTAMP NOT NULL,
    IsActive BOOLEAN NOT NULL,
    IsAutomatic BOOLEAN NOT NULL,
    IsCombinableWithOthers BOOLEAN NOT NULL,
    MaxTotalUsage INT,
    MaxUsagePerUser INT,
    CurrentTotalUsage INT NOT NULL DEFAULT 0,
    MinimumCartAmount DECIMAL(18,2),
    MaximumDiscountAmount DECIMAL(18,2),
    Applicability INT NOT NULL,
    ApplicableProductIds TEXT,
    ApplicableCategoryIds TEXT,
    BuyQuantity INT,
    GetQuantity INT,
    UserId UUID,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL,
    CreatedBy VARCHAR(100) NOT NULL,
    UpdatedBy VARCHAR(100) NOT NULL
);
```

### DiscountUsageHistory Table
```sql
CREATE TABLE DiscountUsageHistories (
    Id UUID PRIMARY KEY,
    DiscountId UUID NOT NULL REFERENCES Discounts(Id),
    UserId UUID NOT NULL,
    OrderId UUID NOT NULL UNIQUE,
    DiscountAmount DECIMAL(18,2) NOT NULL,
    CartTotal DECIMAL(18,2) NOT NULL,
    FinalTotal DECIMAL(18,2) NOT NULL,
    CouponCode VARCHAR(50),
    UsedAt TIMESTAMP NOT NULL,
    UserEmail VARCHAR(255) NOT NULL
);
```

## 🔄 Event-Driven Architecture

The service subscribes to `OrderCreatedEvent` from RabbitMQ to:
- Record discount usage in history
- Update usage counters
- Invalidate relevant cache entries

```csharp
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }
    public decimal CartTotal { get; set; }
    public decimal FinalTotal { get; set; }
    public Guid? DiscountId { get; set; }
    public string? CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime OrderCreatedAt { get; set; }
}
```

## 📈 Performance Optimizations

### Caching Strategy
- **Automatic Discounts**: Cached for 5 minutes
- **Coupon Codes**: Cached for 10 minutes
- **Individual Discounts**: Cached for 15 minutes

### Database Optimizations
- Indexed fields: `CouponCode`, `IsActive`, `IsAutomatic`, `StartDate`, `EndDate`, `UserId`
- Optimized queries with Entity Framework Core
- Connection pooling for PostgreSQL

### Redis Implementation
- JSON serialization for complex objects
- Automatic cache invalidation on updates
- Pattern-based cache clearing

## 🧪 Example API Usage

### Calculate Discount
```bash
curl -X POST "http://localhost:8080/api/discounts/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "123e4567-e89b-12d3-a456-426614174001",
        "productName": "Laptop",
        "categoryId": "123e4567-e89b-12d3-a456-426614174002",
        "categoryName": "Electronics",
        "unitPrice": 1500000,
        "quantity": 1
      }
    ],
    "shippingCost": 50000,
    "couponCode": "SAVE20"
  }'
```

### Create Discount (Admin)
```bash
curl -X POST "http://localhost:8080/api/admin/discounts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "name": "Summer Sale",
    "description": "20% off all electronics",
    "couponCode": "SUMMER20",
    "type": 1,
    "value": 20,
    "startDate": "2024-06-01T00:00:00Z",
    "endDate": "2024-08-31T23:59:59Z",
    "isAutomatic": true,
    "isCombinableWithOthers": false,
    "maxTotalUsage": 1000,
    "minimumCartAmount": 500000,
    "applicability": 3,
    "applicableCategoryIds": ["123e4567-e89b-12d3-a456-426614174002"]
  }'
```

## 🔒 Security Features

- **JWT Authentication**: Secure API access
- **Role-based Authorization**: Admin-only endpoints
- **Input Validation**: Comprehensive request validation
- **SQL Injection Protection**: Parameterized queries
- **CORS Configuration**: Configurable cross-origin requests

## 📋 Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | Required |
| `ConnectionStrings__Redis` | Redis connection | Required |
| `ConnectionStrings__RabbitMQ` | RabbitMQ connection | Required |
| `Jwt__Key` | JWT signing key | Required |
| `Jwt__Issuer` | JWT issuer | Required |
| `Jwt__Audience` | JWT audience | Required |

## 🏥 Health Checks

The service includes health checks for:
- Database connectivity
- Redis connectivity
- Overall service health

Access health check at: `GET /health`

## 📝 Logging

Structured logging with Serilog:
- **Console**: Development and debugging
- **File**: Persistent logs with daily rotation
- **Structured Format**: JSON-compatible logging

## 🚀 Deployment

### Production Deployment
1. Build Docker image: `docker build -t discount-service .`
2. Deploy to container orchestrator (Kubernetes, Docker Swarm)
3. Configure environment variables
4. Set up monitoring and logging aggregation

### Monitoring
- Health checks for container orchestrators
- Structured logs for centralized logging
- Performance metrics through built-in ASP.NET Core metrics

## 🔄 Future Enhancements

- [ ] Discount scheduling with advanced rules
- [ ] A/B testing for discount campaigns
- [ ] Advanced analytics and reporting
- [ ] Machine learning for personalized discounts
- [ ] Integration with external promotion systems
- [ ] Real-time discount recommendations

## 📞 Support

For questions and support:
- Check the API documentation at `/swagger`
- Review the structured logs in the `logs/` directory
- Monitor health endpoints for service status

---

Built with ❤️ using .NET 8 and Clean Architecture principles.
