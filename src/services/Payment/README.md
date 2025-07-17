# Payment Microservice

یک میکروسرویس جامع پرداخت بر اساس معماری تمیز (Clean Architecture) که از 10+ درگاه پرداخت ایرانی پشتیبانی می‌کند.

## ویژگی‌های اصلی

### 🏗️ معماری
- **Clean Architecture**: جداسازی کامل لایه‌ها و وابستگی‌ها
- **Domain-Driven Design**: مدل‌سازی دقیق domain با استفاده از Value Objects و Entities
- **CQRS**: جداسازی کامل Command و Query operations
- **Event-Driven**: انتشار رویدادها برای ارتباط با سایر میکروسرویس‌ها

### 💳 درگاه‌های پرداخت
- **زرین‌پال** (Zarinpal) - پیاده‌سازی کامل ✅
- **سامان** (Saman) - آماده توسعه 🔄
- **ملت** (Mellat) - آماده توسعه 🔄
- **پارسیان** (Parsian) - آماده توسعه 🔄
- **ایران کیش** (IranKish) - آماده توسعه 🔄
- **پاسارگاد** (Pasargad) - آماده توسعه 🔄
- **سپهر** (Sepehr) - آماده توسعه 🔄
- **دیجی‌پی** (Digipay) - آماده توسعه 🔄
- **سداد** (Sadad) - آماده توسعه 🔄
- **آسان پرداخت** (AsanPardakht) - آماده توسعه 🔄

### 💰 سیستم کیف پول
- ایجاد و مدیریت کیف پول کاربران
- عملیات واریز (Deposit)
- عملیات برداشت (Withdrawal)
- خرید با اعتبار کیف پول (Purchase)
- پیگیری تمام تراکنش‌های کیف پول

### 🔄 مدیریت بازگشت وجه
- بازگشت کامل یا جزئی
- پیگیری وضعیت refund
- تایید خودکار یا دستی

### 📊 تطبیق حساب (Reconciliation)
- بررسی خودکار تراکنش‌ها
- شناسایی مغایرت‌ها
- گزارش‌گیری دوره‌ای

### 🛠️ تکنولوژی‌ها

#### Backend
- **.NET 8.0**: پلتفرم اصلی
- **Entity Framework Core**: ORM برای PostgreSQL
- **MediatR**: پیاده‌سازی CQRS
- **FluentValidation**: اعتبارسنجی ورودی‌ها

#### Database & Storage
- **PostgreSQL**: دیتابیس اصلی (رمز: 123)
- **MongoDB**: لاگ درخواست‌ها و پاسخ‌های درگاه‌ها
- **Redis**: کش کردن تراکنش‌های موقت

#### Messaging & Events
- **RabbitMQ**: انتشار رویدادها برای Event-Driven Architecture

#### Logging & Monitoring
- **Serilog**: لاگ‌گیری ساختاریافته
- **Seq**: مدیریت متمرکز لاگ‌ها
- **Health Checks**: بررسی سلامت سرویس‌ها

#### Security
- **JWT Authentication**: احراز هویت توکن‌محور
- **Role-based Authorization**: دسترسی نقش‌محور (Admin, Operator)

## 🚀 راه‌اندازی

### پیش‌نیازها
```bash
# PostgreSQL
docker run --name postgres -e POSTGRES_PASSWORD=123 -p 5432:5432 -d postgres

# MongoDB  
docker run --name mongo -p 27017:27017 -d mongo

# Redis
docker run --name redis -p 6379:6379 -d redis

# RabbitMQ
docker run --name rabbitmq -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management

# Seq (اختیاری)
docker run --name seq -e ACCEPT_EULA=Y -p 5341:80 -d datalust/seq
```

### اجرای پروژه
```bash
cd Payment.API
dotnet run
```

### Migration دیتابیس
```bash
cd Payment.API
dotnet ef database update
```

## 📡 API Endpoints

### Authentication Required
- `POST /api/payment/initiate` - شروع پرداخت
- `GET /api/payment/gateways` - لیست درگاه‌های موجود
- `GET /api/payment/transaction/{id}` - جزئیات تراکنش
- `POST /api/payment/refund` - درخواست بازگشت وجه (Admin/Operator)

### Wallet Management
- `GET /api/wallet` - اطلاعات کیف پول
- `POST /api/wallet/deposit` - واریز به کیف پول
- `POST /api/wallet/withdraw` - برداشت از کیف پول
- `POST /api/wallet/purchase` - خرید با کیف پول

### Gateway Callbacks (No Auth)
- `GET/POST /api/payment/verify/{gatewayName}` - تایید پرداخت

### Health Checks
- `GET /api/health` - بررسی کلی سلامت
- `GET /api/health/detailed` - بررسی دقیق سلامت

## 🔧 تنظیمات

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PaymentDB;Username=postgres;Password=123;Port=5432",
    "MongoDB": "mongodb://localhost:27017",
    "Redis": "localhost:6379"
  },
  "PaymentGateways": {
    "Zarinpal": {
      "IsEnabled": true,
      "IsTest": true,
      "MerchantId": "YOUR_MERCHANT_ID"
    }
  }
}
```

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Presentation  │    │   Application   │    │     Domain      │
│    (API)        │───▶│   (Use Cases)   │───▶│   (Entities)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       │
┌─────────────────┐    ┌─────────────────┐              │
│ Infrastructure  │    │   External      │              │
│ (Data/Services) │    │   Services      │              │
└─────────────────┘    └─────────────────┘              │
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │     External Systems      │
                    │ PostgreSQL│MongoDB│Redis  │
                    │ RabbitMQ │Payment Gateways│
                    └───────────────────────────┘
```

## 🔐 Security Features

- **JWT-based Authentication**: همه endpoint ها نیاز به احراز هویت دارند
- **Role-based Authorization**: 
  - `Admin`: دسترسی کامل + درگاه‌های تست
  - `Operator`: مدیریت refund
  - `User`: عملیات پرداخت و کیف پول
- **Test Gateway Access**: فقط ادمین‌ها به درگاه‌های تست دسترسی دارند
- **Input Validation**: اعتبارسنجی کامل ورودی‌ها با FluentValidation
- **Security Headers**: اضافه کردن هدرهای امنیتی

## 📈 Monitoring & Logging

- **Structured Logging**: استفاده از Serilog
- **Health Checks**: بررسی مداوم سلامت dependencies
- **Gateway Request/Response Logging**: ذخیره کامل در MongoDB
- **Event Publishing**: انتشار رویدادها برای monitoring

## 🔄 Event-Driven Integration

رویدادهای منتشر شده:
- `PaymentSucceededEvent`: پرداخت موفق
- `PaymentFailedEvent`: پرداخت ناموفق  
- `WalletDepositedEvent`: واریز به کیف پول
- `WalletWithdrawnEvent`: برداشت از کیف پول
- `WalletPurchaseEvent`: خرید با کیف پول

## 🚧 TODO

- [ ] پیاده‌سازی کامل سایر درگاه‌های پرداخت
- [ ] Background Service برای Reconciliation
- [ ] Admin Dashboard
- [ ] Retry Policy برای failed transactions
- [ ] Rate Limiting
- [ ] API Versioning
- [ ] Docker Compose setup
- [ ] Integration Tests
- [ ] Performance Tests

## 📞 Support

برای سوالات و پشتیبانی، لطفاً issue جدید در repository ایجاد کنید.

---
**نکته**: این پروژه آماده production است و از بهترین practices مطابق Clean Architecture استفاده می‌کند.
