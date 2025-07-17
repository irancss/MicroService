# مستندات کامل سرویس حمل و نقل

## نمای کلی

این پروژه یک میکروسرویس کامل برای مدیریت حمل و نقل است که با استفاده از معماری CQRS، Clean Architecture و .NET 8 پیاده‌سازی شده است.

## ویژگی‌های پیشرفته

### 🚀 اشتراک‌های ویژه (Premium Subscriptions)
- مدیریت اشتراک‌های ماهانه کاربران
- درخواست‌های ارسال رایگان محدود
- سطح‌بندی کاربران (Basic, Premium, VIP, Enterprise)
- پیگیری استفاده و محدودیت‌ها

### 🎯 قوانین ارسال رایگان (Free Shipping Rules)
- تنظیم داینامیک قوانین توسط ادمین
- شرایط پیچیده قابل تنظیم:
  * مبلغ سفارش
  * تعداد آیتم‌ها
  * وزن کل
  * دسته‌بندی محصولات
  * شهر مقصد
  * روز هفته
  * سطح کاربر
- انواع تخفیف: درصدی، مبلغ ثابت، ارسال کاملاً رایگان

### 🛣️ بهینه‌سازی مسیر (Route Optimization)
- استفاده از Google OR-Tools
- محاسبه بهترین مسیر برای تحویل چندگانه
- برآورد زمان تحویل
- مدیریت راننده‌ها و خودروها

### 📱 رهگیری و اعلان‌رسانی
- رهگیری لحظه‌ای مرسولات
- ارسال SMS از طریق Twilio
- تاریخچه کامل وضعیت‌ها
- اعلان‌های خودکار تغییر وضعیت

### 🔄 مدیریت مرجوعی‌ها
- فرآیند کامل درخواست مرجوعی
- تایید/رد توسط اپراتور
- رهگیری فرآیند مرجوعی
- مدیریت بازپرداخت

## راهنمای استفاده

### 1. تنظیم اشتراک ویژه

```bash
# ایجاد اشتراک ویژه
POST /api/premium-subscriptions
{
  "userId": "user123",
  "subscriptionType": "premium"
}

# بررسی امکان استفاده از ارسال رایگان
GET /api/premium-subscriptions/user/user123/can-use-free-request
```

### 2. تنظیم قوانین ارسال رایگان

```bash
# ایجاد قانون ارسال رایگان برای خرید بالای 500 هزار تومان
POST /api/free-shipping-rules
{
  "name": "ارسال رایگان +500K",
  "discountType": "FreeShipping",
  "discountValue": 100,
  "priority": 1,
  "conditions": [
    {
      "conditionType": "OrderAmount",
      "fieldName": "orderAmount",
      "operator": "GreaterThanOrEqual",
      "value": "500000",
      "valueType": "Decimal",
      "isRequired": true
    }
  ]
}
```

### 3. محاسبه ارسال با قوانین

```bash
# محاسبه هزینه ارسال با اعمال قوانین
POST /api/free-shipping-rules/calculate
{
  "userId": "user123",
  "orderAmount": 750000,
  "itemCount": 3,
  "totalWeight": 2.5,
  "productCategories": ["کتاب", "لوازم تحریر"],
  "shippingMethodId": "uuid",
  "destinationCity": "تهران"
}
```

## نصب و راه‌اندازی

### 1. کلون و نصب

```bash
git clone <repository>
cd Shipping
make restore
```

### 2. تنظیم پایگاه داده

```bash
# ایجاد migration جدید
make migration NAME=AddPremiumFeatures

# اجرای migration
make migrate
```

### 3. تنظیم متغیرهای محیطی

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ShippingServiceDb;..."
  },
  "Twilio": {
    "AccountSid": "your_account_sid",
    "AuthToken": "your_auth_token",
    "FromPhoneNumber": "+1234567890"
  }
}
```

### 4. اجرای پروژه

```bash
make run
# یا
make watch  # برای توسعه
```

## سناریوهای کاربردی

### سناریو 1: اعمال ارسال رایگان هوشمند

```csharp
// کاربر با اشتراک Premium، سفارش 400 هزار تومان کتاب
var shipmentData = new ShipmentData
{
    UserId = "premium_user",
    OrderAmount = 400000,
    ProductCategories = new[] { "کتاب" },
    DestinationCity = "تهران"
};

// بررسی قوانین ارسال رایگان
var (rule, discount) = await _freeShippingService
    .CalculateFreeShippingAsync(shipmentData, isPremiumUser: true);

// بررسی امکان استفاده از اشتراک ویژه
if (discount == 0 && await _subscriptionService.CanUseFreeRequestAsync(userId))
{
    await _subscriptionService.UseFreeRequestAsync(userId, shipmentId, originalCost);
    discount = originalCost; // ارسال کاملاً رایگان
}
```

### سناریو 2: قوانین شرطی پیشرفته

```json
{
  "name": "ارسال رایگان ویژه تهران - جمعه‌ها",
  "conditions": [
    {
      "conditionType": "DestinationCity",
      "fieldName": "destinationCity", 
      "operator": "Equals",
      "value": "تهران",
      "isRequired": true
    },
    {
      "conditionType": "DayOfWeek",
      "fieldName": "dayOfWeek",
      "operator": "Equals", 
      "value": "Friday",
      "isRequired": true
    },
    {
      "conditionType": "OrderAmount",
      "fieldName": "orderAmount",
      "operator": "GreaterThan",
      "value": "200000",
      "isRequired": false
    }
  ]
}
```

## API Endpoints کامل

### اشتراک‌های ویژه
- `GET /api/premium-subscriptions/user/{userId}/active` - اشتراک فعال
- `POST /api/premium-subscriptions` - ایجاد اشتراک
- `PUT /api/premium-subscriptions/{id}/extend` - تمدید اشتراک
- `PUT /api/premium-subscriptions/{id}/cancel` - لغو اشتراک
- `POST /api/premium-subscriptions/use-free-request` - استفاده از درخواست رایگان
- `GET /api/premium-subscriptions/{id}/usage-history` - تاریخچه استفاده

### قوانین ارسال رایگان
- `GET /api/free-shipping-rules/active` - قوانین فعال
- `POST /api/free-shipping-rules` - ایجاد قانون
- `PUT /api/free-shipping-rules/{id}` - ویرایش قانون
- `DELETE /api/free-shipping-rules/{id}` - حذف قانون
- `POST /api/free-shipping-rules/calculate` - محاسبه تخفیف
- `POST /api/free-shipping-rules/{id}/conditions` - افزودن شرط
- `PUT /api/free-shipping-rules/{id}/status` - تغییر وضعیت

### مرسولات پیشرفته
- `GET /api/shipments` - لیست مرسولات
- `POST /api/shipments` - ایجاد مرسوله
- `GET /api/shipments/{id}/tracking` - رهگیری
- `PUT /api/shipments/{id}/status` - تغییر وضعیت
- `POST /api/shipments/{id}/optimize-route` - بهینه‌سازی مسیر

### مرجوعی‌ها
- `GET /api/returns` - لیست مرجوعی‌ها
- `POST /api/returns` - درخواست مرجوعی
- `PUT /api/returns/{id}/approve` - تایید مرجوعی
- `PUT /api/returns/{id}/reject` - رد مرجوعی
- `GET /api/returns/{id}/tracking` - رهگیری مرجوعی

## تست‌ها

### اجرای تست‌ها

```bash
# تمام تست‌ها
make test

# تست‌های واحد
dotnet test --filter Category=Unit

# تست‌های یکپارچگی
dotnet test --filter Category=Integration

# Coverage report
make test-coverage
```

### نمونه تست‌ها

```csharp
[Test]
public async Task FreeShippingRule_ShouldApplyCorrectDiscount()
{
    // Arrange
    var rule = CreateTestRule();
    var shipmentData = CreateTestShipmentData();
    
    // Act
    var discount = rule.CalculateDiscount(1000);
    
    // Assert
    Assert.AreEqual(expectedDiscount, discount);
}
```

## مانیتورینگ

### Health Checks
- `GET /health` - وضعیت سرویس
- `GET /health/ready` - آمادگی سرویس
- `GET /health/live` - زنده بودن سرویس

### Metrics
- تعداد اشتراک‌های فعال
- استفاده از قوانین ارسال رایگان
- زمان پاسخ‌دهی API
- تعداد خطاها

## امنیت

### احراز هویت
- JWT Token validation
- Role-based authorization
- Rate limiting

### داده‌ها
- رمزنگاری اطلاعات حساس
- Audit logging
- Data retention policies

## مشارکت

1. Fork کردن پروژه
2. ایجاد feature branch
3. نوشتن تست‌ها
4. ارسال Pull Request

## پشتیبانی

- GitHub Issues برای گزارش باگ
- مستندات API در Swagger
- لاگ‌های سیستم در `/logs`
