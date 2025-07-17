# 🚢 Shipping Microservice - میکروسرویس حمل و نقل

یک میکروسرویس کامل و پیشرفته برای مدیریت حمل و نقل با معماری CQRS، ویژگی‌های اشتراک ویژه، و قوانین ارسال رایگان داینامیک.

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Supported-blue.svg)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 🌟 ویژگی‌های اصلی

### 🏗️ معماری پیشرفته
- **Clean Architecture** با جداسازی کامل لایه‌ها
- **CQRS Pattern** با استفاده از MediatR 12.2.0
- **Domain-Driven Design (DDD)** با Aggregates و Value Objects
- **Repository Pattern** برای دسترسی به داده
- **Dependency Injection** کامل در تمام لایه‌ها
- **Event Sourcing** برای ردیابی تغییرات

### 📋 قابلیت‌های تجاری
- **مدیریت روش‌های حمل و نقل** (Shipping Methods)
- **قوانین قیمت‌گذاری پیشرفته** (Cost Rules)
- **قوانین محدودیت** (Restriction Rules) 
- **مدیریت بازه‌های زمانی** (Time Slots)
- **جستجوی هوشمند** گزینه‌های حمل و نقل موجود
- **رهگیری مرسولات** با اعلان‌های SMS
- **مدیریت مرجوعی‌ها** و تعویض‌ها
- **بهینه‌سازی مسیر** با Google OR-Tools

### � ویژگی‌های اشتراک ویژه (Premium Features)
- **سیستم اشتراک ویژه** با مدیریت ماهانه
- **قوانین ارسال رایگان داینامیک** قابل تنظیم توسط ادمین
- **شرایط پیچیده** برای اعمال تخفیف (مبلغ، وزن، دسته‌بندی، تعداد)
- **محدودیت درخواست‌های رایگان** برای اشتراک‌های ویژه
- **ردیابی استفاده** از ویژگی‌های ویژه
- **مدیریت چندین شرط** همزمان با اولویت‌بندی

### �🛠️ تکنولوژی‌ها
- **.NET 8** - آخرین نسخه پلتفرم توسعه
- **ASP.NET Core 8** - وب API با عملکرد بالا
- **Entity Framework Core 8.0** - ORM پیشرفته
- **PostgreSQL 15** - پایگاه داده قدرتمند
- **MediatR 12.2.0** - الگوی CQRS و Mediator
- **FluentValidation 11.8.0** - اعتبارسنجی پیشرفته
- **AutoMapper 12.0.1** - نگاشت اشیاء
- **Polly 8.2.0** - مقاومت در برابر خرابی
- **Google OR-Tools 9.8.3296** - بهینه‌سازی مسیر
- **Twilio 7.2.2** - ارسال اعلان SMS
- **Swagger/OpenAPI** - مستندات کامل API

### 🔧 ابزارهای توسعه
- **Docker & Docker Compose** - کانتینریزیشن کامل
- **Makefile** - خودکارسازی فرآیندهای ساخت
- **GitHub Actions** - CI/CD پایپ‌لاین
- **Prometheus & Grafana** - مانیتورینگ
- **Jaeger** - Distributed Tracing
- **Redis** - کش و Session Management

## 🚀 راه‌اندازی سریع

### 📋 پیش‌نیازها
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker & Docker Compose](https://www.docker.com/get-started) (اختیاری)
- [Git](https://git-scm.com/downloads)

### ⚡ نصب و راه‌اندازی

#### روش 1: استفاده از Makefile (توصیه می‌شود)
```bash
# راه‌اندازی اولیه محیط توسعه
make first-run

# اجرای سرور توسعه
make dev-run
```

#### روش 2: راه‌اندازی دستی
```bash
# 1. بازگردانی پکیج‌ها
dotnet restore Shipping.sln

# 2. ساخت پروژه
dotnet build Shipping.sln --configuration Release

# 3. تنظیم متغیرهای محیط
cp .env.example .env
# ویرایش فایل .env با مقادیر صحیح

# 4. اجرای مایگریشن‌ها
dotnet ef database update --project ShippingService.Infrastructure --startup-project ShippingService.API

# 5. اعمال ویژگی‌های ویژه
make premium-migrate

# 6. اجرای اپلیکیشن
dotnet run --project ShippingService.API
```

#### روش 3: استفاده از Docker
```bash
# ساخت و اجرای با Docker Compose
docker-compose up -d

# یا استفاده از Makefile
make docker-compose
```

## 📖 مستندات کامل

### 🏛️ ساختار پروژه
```
📁 ShippingService/
├── 📁 ShippingService.API/           # لایه API و Controllers
│   ├── 📁 Controllers/               # کنترلرهای API
│   ├── 📁 Middleware/               # میدل‌ویرهای سفارشی
│   └── 📄 Program.cs                # نقطه ورود اپلیکیشن
├── 📁 ShippingService.Application/   # لایه Application و CQRS
│   ├── 📁 Commands/                 # Command Handlers
│   ├── 📁 Queries/                  # Query Handlers
│   ├── 📁 Services/                 # سرویس‌های تجاری
│   └── 📁 Validators/               # اعتبارسنجی‌ها
├── 📁 ShippingService.Domain/        # لایه Domain و Entities
│   ├── 📁 Entities/                 # موجودیت‌های اصلی
│   ├── 📁 ValueObjects/             # Value Objects
│   ├── 📁 Events/                   # Domain Events
│   └── 📁 Enums/                    # تعریف Enums
├── 📁 ShippingService.Infrastructure/ # لایه Infrastructure
│   ├── 📁 Data/                     # DbContext و Configurations
│   ├── 📁 Repositories/             # پیاده‌سازی Repository ها
│   └── 📁 Services/                 # سرویس‌های خارجی
├── 📄 Dockerfile                    # تعریف Container
├── 📄 docker-compose.yml            # تنظیمات Multi-Container
├── 📄 Makefile                      # خودکارسازی فرآیندها
└── 📄 MIGRATION_PREMIUM_FEATURES.sql # Migration دستی ویژگی‌های ویژه
```

### 🎯 API Endpoints

#### 🚢 Shipping Methods
```http
GET    /api/shipping-methods              # دریافت تمام روش‌های حمل و نقل
POST   /api/shipping-methods              # ایجاد روش حمل و نقل جدید
GET    /api/shipping-methods/{id}         # دریافت روش حمل و نقل خاص
PUT    /api/shipping-methods/{id}         # به‌روزرسانی روش حمل و نقل
DELETE /api/shipping-methods/{id}         # حذف روش حمل و نقل
```

#### 💎 Premium Subscriptions
```http
GET    /api/premium-subscriptions                    # دریافت تمام اشتراک‌های ویژه
POST   /api/premium-subscriptions                    # ایجاد اشتراک ویژه جدید
GET    /api/premium-subscriptions/{id}               # دریافت اشتراک ویژه خاص
PUT    /api/premium-subscriptions/{id}/extend        # تمدید اشتراک ویژه
DELETE /api/premium-subscriptions/{id}/cancel       # لغو اشتراک ویژه
POST   /api/premium-subscriptions/{id}/use-request   # استفاده از درخواست رایگان
```

#### 🎁 Free Shipping Rules
```http
GET    /api/free-shipping-rules           # دریافت تمام قوانین ارسال رایگان
POST   /api/free-shipping-rules           # ایجاد قانون ارسال رایگان جدید
GET    /api/free-shipping-rules/{id}      # دریافت قانون خاص
PUT    /api/free-shipping-rules/{id}      # به‌روزرسانی قانون
DELETE /api/free-shipping-rules/{id}      # حذف قانون
POST   /api/free-shipping-rules/{id}/apply # اعمال قانون به سفارش
```

#### 📊 Cost Calculation
```http
POST   /api/shipping/calculate-cost       # محاسبه هزینه حمل و نقل
POST   /api/shipping/optimize-route       # بهینه‌سازی مسیر تحویل
```

#### 📱 Tracking & Notifications
```http
GET    /api/tracking/{trackingNumber}     # ردیابی مرسوله
POST   /api/tracking/{id}/update-status   # به‌روزرسانی وضعیت
POST   /api/notifications/send            # ارسال اعلان
```

### 🔧 تنظیمات Makefile

فایل Makefile شامل دستورات کاملی برای مدیریت پروژه است:

#### دستورات اصلی
```bash
make help           # نمایش کامل راهنما
make clean          # پاک‌سازی فایل‌های ساخت
make build          # ساخت پروژه
make test           # اجرای تست‌ها
make dev-run        # اجرای سرور توسعه
```

#### دستورات دیتابیس
```bash
make migrate        # اجرای migration ها
make migration      # ایجاد migration جدید
make db-seed        # اجرای seed data
make premium-migrate # اعمال ویژگی‌های ویژه
```

#### دستورات Docker
```bash
make docker-build   # ساخت تصویر Docker
make docker-run     # اجرای کانتینر
make docker-compose # اجرای با docker-compose
```

### 🔒 تنظیمات امنیتی

#### JWT Authentication
```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "ShippingService",
    "Audience": "ShippingServiceClients",
    "ExpiryMinutes": 60
  }
}
```

#### Rate Limiting
```json
{
  "RateLimiting": {
    "Enabled": true,
    "RequestsPerMinute": 100,
    "BurstSize": 50
  }
}
```

### 📊 نظارت و مانیتورینگ

#### Health Checks
- `/health` - بررسی سلامت کلی
- `/health/ready` - بررسی آمادگی
- `/health/live` - بررسی زنده بودن

#### Metrics (Prometheus)
- `http_requests_total` - تعداد کل درخواست‌ها
- `http_request_duration_seconds` - مدت زمان درخواست‌ها
- `shipping_calculations_total` - تعداد محاسبات حمل و نقل
- `premium_subscriptions_active` - تعداد اشتراک‌های ویژه فعال

#### Logging
- **Structured Logging** با Serilog
- **Log Levels**: Trace, Debug, Information, Warning, Error, Critical
- **Log Sinks**: Console, File, Elasticsearch (اختیاری)

## 🧪 تست‌ها

### اجرای تست‌ها
```bash
# تمام تست‌ها
make test

# تست‌های ویژگی‌های ویژه
make premium-test

# تست‌ها با گزارش پوشش
make coverage
```

### انواع تست‌ها
- **Unit Tests** - تست واحد برای منطق تجاری
- **Integration Tests** - تست یکپارچگی API
- **Performance Tests** - تست عملکرد
- **Load Tests** - تست بار

## 🔄 CI/CD

### GitHub Actions Pipeline
```yaml
# .github/workflows/ci.yml
name: CI/CD Pipeline
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Run CI Pipeline
        run: make ci-build
```

### دستورات CI/CD
```bash
make ci-build       # پایپ‌لاین ساخت CI
make cd-deploy      # پایپ‌لاین استقرار CD
```

## 🐳 استقرار با Docker

### تنظیمات Production
```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  shipping-api:
    image: your-registry.com/shipping-service:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${PROD_DB_CONNECTION}
    deploy:
      replicas: 3
      resources:
        limits:
          memory: 512M
        reservations:
          memory: 256M
```

### استقرار با Kubernetes
```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: shipping-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: shipping-service
  template:
    spec:
      containers:
      - name: shipping-api
        image: shipping-service:latest
        ports:
        - containerPort: 80
```

## 🤝 مشارکت

برای مشارکت در این پروژه:

1. **Fork** کنید
2. یک **branch** جدید ایجاد کنید (`git checkout -b feature/amazing-feature`)
3. تغییرات را **commit** کنید (`git commit -m 'Add some amazing feature'`)
4. **Push** کنید (`git push origin feature/amazing-feature`)
5. یک **Pull Request** ایجاد کنید

### استانداردهای کد
- کد باید فرمت شده باشد (`make format`)
- تمام تست‌ها باید پاس شوند (`make test`)
- Coverage باید بالای 80% باشد
- مستندات باید به‌روزرسانی شود

## 📄 مجوز

این پروژه تحت مجوز MIT منتشر شده است. فایل [LICENSE](LICENSE) را برای جزئیات بیشتر مطالعه کنید.

## 📞 پشتیبانی

برای پشتیبانی و سوالات:
- **Issues**: [GitHub Issues](https://github.com/your-repo/shipping-service/issues)
- **Documentation**: [Wiki](https://github.com/your-repo/shipping-service/wiki)
- **Email**: support@yourcompany.com

## 🗺️ نقشه راه آینده

- [ ] **GraphQL API** پیاده‌سازی
- [ ] **Event Sourcing** کامل
- [ ] **Multi-tenant** پشتیبانی
- [ ] **Machine Learning** برای پیش‌بینی زمان تحویل
- [ ] **Mobile SDK** برای iOS و Android
- [ ] **Webhook** سیستم برای اعلان‌های Real-time

---

<div align="center">

**ساخته شده با ❤️ توسط تیم توسعه ShippingService**

[🌟 Star این پروژه](https://github.com/your-repo/shipping-service) | [🐛 گزارش Bug](https://github.com/your-repo/shipping-service/issues) | [💡 درخواست Feature](https://github.com/your-repo/shipping-service/issues)

</div>

4. **اجرای پروژه**
```bash
cd ShippingService.API
dotnet run
```

## API Endpoints

### مدیریت روش‌های حمل و نقل
- `POST /api/shipping/methods` - ایجاد روش حمل و نقل جدید
- `PUT /api/shipping/methods/{id}` - ویرایش روش حمل و نقل
- `DELETE /api/shipping/methods/{id}` - حذف روش حمل و نقل
- `GET /api/shipping/methods` - دریافت لیست روش‌های حمل و نقل
- `GET /api/shipping/available-options` - دریافت گزینه‌های موجود

### مدیریت بازه‌های زمانی
- `POST /api/timeslots/templates` - ایجاد قالب بازه زمانی
- `POST /api/timeslots/book` - رزرو بازه زمانی
- `GET /api/timeslots/available` - دریافت بازه‌های موجود
- `DELETE /api/timeslots/bookings/{id}` - لغو رزرو

### Health Check
- `GET /api/health` - بررسی سلامت سرویس

## Status: ✅ COMPLETED
- کامل پیاده‌سازی شده با معماری CQRS
- تمام لایه‌ها (Domain, Application, Infrastructure, API) آماده
- Database Migrations ایجاد شده
- API Controllers و endpoints کامل
- Business Logic پیشرفته برای قیمت‌گذاری و محدودیت‌ها