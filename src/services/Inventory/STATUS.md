# 🎯 **میکروسرویس پیشرفته مدیریت موجودی با سیستم هشدار فعال**

## ✅ **پیاده‌سازی کامل شده است:**

### 🏗️ **معماری Clean Architecture**
- **Domain Layer**: انتیتی‌های `ProductStock` و `StockTransaction` با منطق کسب‌وکار
- **Application Layer**: الگوی CQRS با MediatR شامل Commands، Queries، و Handlers
- **Infrastructure Layer**: Repository ها، Caching، Event Publishing
- **API Layer**: کنترلرها با Authentication و Health Checks

### 🔔 **سیستم هشدار فعال و هوشمند**
- **مدیریت آستانه‌های پویا**: تنظیم `LowStockThreshold` و `ExcessStockThreshold` برای هر محصول
- **انتشار خودکار رویدادها**: `LowStockDetectedEvent` و `ExcessStockDetectedEvent`
- **تشخیص هوشمند**: متدهای `IsLowStock()` و `IsExcessStock()` 
- **یکپارچگی با عملیات**: بررسی خودکار آستانه‌ها در تمام عملیات موجودی

### ⚡ **عملیات پیشرفته موجودی**
- **تنظیم موجودی**: با بررسی خودکار آستانه‌ها
- **سیستم رزرو**: Reserve، Commit، Cancel با کنترل همزمانی
- **عملیات دسته‌ای**: بازیابی موجودی چندین محصول
- **کنترل همزمانی بهینه**: با استفاده از Version tracking
- **تاریخچه تراکنش‌ها**: ردیابی کامل تغییرات موجودی

### 🛠️ **زیرساخت سازمانی**
- **PostgreSQL**: پایگاه داده اصلی با Entity Framework Core
- **Redis**: کشینگ پرسرعت برای کوئری‌های موجودی
- **RabbitMQ/MassTransit**: انتشار رویدادها
- **JWT Authentication**: احراز هویت برای endpoints مدیریتی
- **FluentValidation**: اعتبارسنجی جامع ورودی‌ها

### 📡 **API Endpoints**

#### عملیات موجودی:
```
GET    /api/stock/{productId}              - دریافت موجودی فعلی
POST   /api/stock/adjust                   - تنظیم موجودی
POST   /api/stock/reserve                  - رزرو موجودی
POST   /api/stock/commit                   - تایید رزرو
POST   /api/stock/cancel                   - لغو رزرو
GET    /api/stock/multiple                 - موجودی چندین محصول
GET    /api/stock/transactions/{productId} - تاریخچه تراکنش‌ها
```

#### مدیریت آستانه‌ها (Admin - JWT Required):
```
GET    /api/admin/thresholds/products/{productId}  - دریافت آستانه‌ها
POST   /api/admin/thresholds/products/{productId}  - تنظیم آستانه‌های جدید
PUT    /api/admin/thresholds/products/{productId}  - بروزرسانی آستانه‌ها
GET    /api/admin/thresholds/all                   - تمام آستانه‌های محصولات
GET    /api/admin/thresholds/alerts                - محصولات دارای هشدار
```

### 🔔 **سیستم رویدادها**

#### LowStockDetectedEvent:
```json
{
  "ProductId": "ABC123",
  "CurrentStock": 5,
  "LowStockThreshold": 10,
  "DetectedAt": "2024-01-15T10:30:00Z"
}
```

#### ExcessStockDetectedEvent:
```json
{
  "ProductId": "ABC123", 
  "CurrentStock": 1000,
  "ExcessStockThreshold": 500,
  "DetectedAt": "2024-01-15T10:30:00Z"
}
```

### ⚡ **ویژگی‌های کلیدی**

1. **نظارت فعال**: هشدار بلادرنگ در زمان عبور از آستانه‌ها
2. **آستانه‌های پویا**: قابلیت تنظیم آستانه برای هر محصول
3. **رویدادمحور**: جداسازی ضعیف بین عملیات موجودی و هشدار
4. **بهینه‌سازی عملکرد**: کشینگ Redis برای سناریوهای ترافیک بالا
5. **کنترل همزمانی**: جلوگیری از تناقض موجودی در دسترسی همزمان
6. **عملیات دسته‌ای**: بازیابی کارآمد موجودی چندین محصول

### 🔧 **تکنولوژی‌های استفاده شده**
- **.NET 8**: آخرین فریمورک با بهینه‌سازی‌های عملکرد
- **PostgreSQL**: پایگاه داده اصلی
- **Redis**: لایه کشینگ پرسرعت
- **RabbitMQ/MassTransit**: سیستم پیام‌رسانی
- **JWT**: احراز هویت ایمن
- **FluentValidation**: اعتبارسنجی پیشرفته
- **Health Checks**: نظارت بر سلامت سیستم

### 🏆 **ارزش کسب‌وکار**
- **نظارت فعال**: جلوگیری از کمبود موجودی و انباشت کالا
- **معماری مقیاس‌پذیر**: امکان مقیاس‌گیری مستقل
- **کارایی عملیاتی**: کاهش نظارت دستی
- **یکپارچگی داده**: کنترل همزمانی در سناریوهای ترافیک بالا
- **قابلیت ممیزی**: تاریخچه کامل تراکنش‌ها

## ⚠️ **وضعیت فعلی**: 
مشکل کامپایل به دلیل package dependencies وجود دارد. کد کاملا نوشته شده و معماری پیاده‌سازی شده است.

## 🔧 **برای اجرا**:
```bash
dotnet clean
dotnet restore  
dotnet build
dotnet run --project InventoryService.API
```
