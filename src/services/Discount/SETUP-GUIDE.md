# راهنمای راه‌اندازی سرویس تخفیف (Discount Service)

## 📋 فهرست مطالب
- [پیش‌نیازها](#پیش‌نیازها)
- [راه‌اندازی سریع](#راه‌اندازی-سریع)
- [راه‌اندازی توسعه](#راه‌اندازی-توسعه)
- [راه‌اندازی تولید](#راه‌اندازی-تولید)
- [دستورات مفید](#دستورات-مفید)
- [عیب‌یابی](#عیب‌یابی)

## پیش‌نیازها

### نرم‌افزارهای مورد نیاز:
- **.NET 8 SDK** - [دانلود](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker & Docker Compose** - [دانلود](https://www.docker.com/products/docker-desktop)
- **PostgreSQL** (اختیاری - اگر از Docker استفاده نمی‌کنید)
- **Redis** (اختیاری - اگر از Docker استفاده نمی‌کنید)
- **RabbitMQ** (اختیاری - اگر از Docker استفاده نمی‌کنید)

### بررسی نصب:
```bash
# بررسی .NET
dotnet --version

# بررسی Docker
docker --version
docker-compose --version

# بررسی Make (Windows: استفاده از WSL یا Git Bash)
make --version
```

## راه‌اندازی سریع

### 1. دانلود و راه‌اندازی کامل:
```bash
# کلون پروژه
git clone <repository-url>
cd discount-service

# راه‌اندازی کامل با یک دستور
make full-setup
```

### 2. دسترسی به سرویس:
- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

## راه‌اندازی توسعه

### 1. راه‌اندازی محیط توسعه:

```bash
# 1. کپی کردن فایل تنظیمات
make env-copy

# 2. ویرایش تنظیمات (اختیاری)
# nano .env

# 3. راه‌اندازی پایگاه داده و سرویس‌های جانبی
make docker-dev

# 4. اعمال migrations
make migrate

# 5. اجرای API
make run-watch
```

### 2. جریان کار روزانه:

```bash
# شروع روز
make docker-dev      # شروع سرویس‌های جانبی
make run-watch       # اجرای API با hot reload

# توسعه
make test           # اجرای تست‌ها
make format         # فرمت کردن کد
make lint          # بررسی کیفیت کد

# پایان روز
make docker-stop    # توقف سرویس‌ها
```

### 3. کار با دیتابیس:

```bash
# اضافه کردن migration جدید
make migrate-add NAME=AddNewFeature

# اعمال migrations
make migrate

# حذف آخرین migration
make migrate-remove

# ریست کامل دیتابیس
make db-reset
```

## راه‌اندازی تولید

### 1. با Docker:

```bash
# ساخت image تولیدی
make prod-docker

# اجرا در محیط تولید
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=prod-db;Database=DiscountDb;Username=user;Password=pass" \
  -e ConnectionStrings__Redis="prod-redis:6379" \
  -e ConnectionStrings__RabbitMQ="rabbitmq://user:pass@prod-rabbitmq:5672/" \
  discount-service:prod
```

### 2. با Docker Compose:

```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  discount-api:
    image: discount-service:prod
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=DiscountDb;Username=postgres;Password=prod_password
    ports:
      - "8080:8080"
    depends_on:
      - postgres
      - redis
      - rabbitmq
```

### 3. متغیرهای محیطی تولید:

```bash
# متغیرهای ضروری
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=prod-db;Database=DiscountDb;Username=user;Password=secure_password"
export ConnectionStrings__Redis="prod-redis:6379"
export ConnectionStrings__RabbitMQ="rabbitmq://user:secure_password@prod-rabbitmq:5672/"
export Jwt__Key="YourVerySecureProductionJwtKey256Bits!"
```

## دستورات مفید

### مدیریت عمومی:
```bash
make help           # نمایش تمام دستورات
make status         # وضعیت سرویس‌ها
make info          # اطلاعات پروژه
make health        # بررسی سلامت API
```

### توسعه:
```bash
make setup         # راه‌اندازی اولیه
make build         # ساخت پروژه
make test          # اجرای تست‌ها
make run           # اجرای API
make run-watch     # اجرای API با hot reload
```

### Docker:
```bash
make docker-build  # ساخت Docker image
make docker-run    # اجرای کامل با Docker
make docker-dev    # اجرای سرویس‌های جانبی
make docker-logs   # نمایش لاگ‌ها
make docker-stop   # توقف سرویس‌ها
make docker-clean  # پاک‌سازی Docker
```

### دیتابیس:
```bash
make migrate                    # اعمال migrations
make migrate-add NAME=MyChange  # اضافه کردن migration
make migrate-remove            # حذف آخرین migration
make db-reset                  # ریست کامل دیتابیس
```

## عیب‌یابی

### مشکلات رایج:

#### 1. سرویس اجرا نمی‌شود:
```bash
# بررسی وضعیت
make status

# بررسی لاگ‌ها
make docker-logs

# راه‌حل: ریستارت کامل
make docker-clean
make full-setup
```

#### 2. مشکل اتصال به دیتابیس:
```bash
# بررسی وضعیت PostgreSQL
docker-compose exec postgres pg_isready -U postgres

# راه‌حل: ریستارت دیتابیس
docker-compose restart postgres
make migrate
```

#### 3. مشکل Redis:
```bash
# بررسی Redis
docker-compose exec redis redis-cli ping

# راه‌حل: ریستارت Redis
docker-compose restart redis
```

#### 4. مشکل RabbitMQ:
```bash
# بررسی RabbitMQ Management
# http://localhost:15672 (guest/guest)

# راه‌حل: ریستارت RabbitMQ
docker-compose restart rabbitmq
```

### لاگ‌ها و نظارت:

```bash
# مشاهده لاگ‌های زنده
make docker-logs

# لاگ‌های فایل
tail -f logs/discount-service-*.log

# Health check
make health

# وضعیت کامل
make troubleshoot
```

### بررسی عملکرد:

```bash
# تست API
curl -X POST http://localhost:8080/api/discounts/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "items": [
      {
        "productId": "123e4567-e89b-12d3-a456-426614174001",
        "productName": "Test Product",
        "categoryId": "123e4567-e89b-12d3-a456-426614174002",
        "categoryName": "Test Category",
        "unitPrice": 100000,
        "quantity": 1
      }
    ],
    "shippingCost": 10000
  }'
```

## نکات امنیتی

### 1. تولید:
- JWT Key را تغییر دهید
- رمزهای پیش‌فرض را تغییر دهید
- HTTPS را فعال کنید
- فایروال را تنظیم کنید

### 2. Development:
- از رمزهای قوی استفاده کنید
- .env را در .gitignore قرار دهید
- دسترسی‌های غیرضروری را محدود کنید

## پشتیبانی

### مشکل داری؟
1. [مستندات API](http://localhost:8080/swagger) را بررسی کن
2. [لاگ‌های سیستم](logs/) را چک کن
3. `make troubleshoot` را اجرا کن
4. از GitHub Issues استفاده کن

### منابع مفید:
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Redis Documentation](https://redis.io/documentation)
- [RabbitMQ Documentation](https://www.rabbitmq.com/documentation.html)

---
**نکته**: این راهنما برای سیستم‌عامل‌های Unix-like (Linux/macOS) و Windows با WSL نوشته شده است.
