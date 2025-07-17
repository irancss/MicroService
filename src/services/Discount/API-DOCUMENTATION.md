# مستندات API سرویس تخفیف

## 📖 معرفی

سرویس تخفیف یک میکروسرویس کامل برای مدیریت و محاسبه انواع تخفیف‌ها در پلتفرم‌های تجارت الکترونیک است.

### ویژگی‌های کلیدی:
- پشتیبانی از انواع مختلف تخفیف
- سیستم کد تخفیف پیشرفته
- محدودیت‌های زمانی و کاربری
- کش هوشمند برای عملکرد بالا
- معماری event-driven

## 🔗 Base URL
```
Development: http://localhost:8080
Production: https://api.yourcompany.com/discount
```

## 🔐 احراز هویت

### JWT Token
تمام endpoint های محافظت شده نیاز به JWT token دارند:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

### نقش‌های کاربری:
- **Admin**: دسترسی کامل به مدیریت تخفیف‌ها
- **User**: دسترسی به تاریخچه شخصی
- **Anonymous**: فقط محاسبه تخفیف

## 📋 Endpoints

### 1. محاسبه تخفیف

#### `POST /api/discounts/calculate`
محاسبه بهترین تخفیف ممکن برای سبد خرید

**Request Body:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "items": [
    {
      "productId": "123e4567-e89b-12d3-a456-426614174001",
      "productName": "لپ‌تاپ گیمینگ",
      "categoryId": "123e4567-e89b-12d3-a456-426614174002",
      "categoryName": "لپ‌تاپ",
      "unitPrice": 15000000,
      "quantity": 1
    },
    {
      "productId": "123e4567-e89b-12d3-a456-426614174003",
      "productName": "ماوس گیمینگ",
      "categoryId": "123e4567-e89b-12d3-a456-426614174004",
      "categoryName": "لوازم جانبی",
      "unitPrice": 500000,
      "quantity": 2
    }
  ],
  "shippingCost": 50000,
  "couponCode": "SUMMER20"
}
```

**Response:**
```json
{
  "originalTotal": 16050000,
  "discountAmount": 3000000,
  "finalTotal": 13050000,
  "discountDescription": "20% تخفیف تابستانی",
  "appliedDiscountId": "123e4567-e89b-12d3-a456-426614174005",
  "couponCode": "SUMMER20",
  "isSuccess": true,
  "errorMessage": "",
  "shippingDiscount": 0,
  "appliedDiscounts": [
    {
      "discountId": "123e4567-e89b-12d3-a456-426614174005",
      "name": "تخفیف تابستانی",
      "amount": 3000000,
      "description": "20% تخفیف برای تمام محصولات",
      "couponCode": "SUMMER20"
    }
  ]
}
```

### 2. مدیریت تخفیف‌ها (Admin)

#### `POST /api/admin/discounts`
ایجاد تخفیف جدید

**Headers:**
```
Authorization: Bearer ADMIN_JWT_TOKEN
```

**Request Body:**
```json
{
  "name": "تخفیف عید نوروز",
  "description": "30% تخفیف ویژه عید نوروز برای تمام محصولات الکترونیکی",
  "couponCode": "NOWRUZ30",
  "type": 1,
  "value": 30,
  "startDate": "2024-03-20T00:00:00Z",
  "endDate": "2024-03-27T23:59:59Z",
  "isAutomatic": false,
  "isCombinableWithOthers": false,
  "maxTotalUsage": 1000,
  "maxUsagePerUser": 1,
  "minimumCartAmount": 1000000,
  "maxDiscountAmount": 5000000,
  "applicability": 3,
  "applicableCategoryIds": [
    "123e4567-e89b-12d3-a456-426614174002"
  ],
  "buyQuantity": null,
  "getQuantity": null,
  "userId": null
}
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174006",
  "name": "تخفیف عید نوروز",
  "description": "30% تخفیف ویژه عید نوروز برای تمام محصولات الکترونیکی",
  "couponCode": "NOWRUZ30",
  "type": 1,
  "value": 30,
  "startDate": "2024-03-20T00:00:00Z",
  "endDate": "2024-03-27T23:59:59Z",
  "isActive": true,
  "isAutomatic": false,
  "isCombinableWithOthers": false,
  "maxTotalUsage": 1000,
  "maxUsagePerUser": 1,
  "currentTotalUsage": 0,
  "minimumCartAmount": 1000000,
  "maxDiscountAmount": 5000000,
  "applicability": 3,
  "applicableProductIds": null,
  "applicableCategoryIds": [
    "123e4567-e89b-12d3-a456-426614174002"
  ],
  "buyQuantity": null,
  "getQuantity": null,
  "userId": null,
  "createdAt": "2024-03-15T10:30:00Z",
  "updatedAt": "2024-03-15T10:30:00Z",
  "createdBy": "admin@company.com",
  "updatedBy": "admin@company.com"
}
```

#### `GET /api/admin/discounts/{id}`
دریافت تخفیف با شناسه

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174006",
  "name": "تخفیف عید نوروز",
  // ... سایر فیلدها
}
```

#### `GET /api/admin/discounts`
دریافت لیست صفحه‌بندی شده تخفیف‌ها

**Query Parameters:**
- `pageNumber` (int): شماره صفحه (پیش‌فرض: 1)
- `pageSize` (int): اندازه صفحه (پیش‌فرض: 10)
- `searchTerm` (string): عبارت جستجو

**Response:**
```json
{
  "discounts": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174006",
      "name": "تخفیف عید نوروز",
      // ... سایر فیلدها
    }
  ],
  "totalCount": 25,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

#### `PUT /api/admin/discounts/{id}`
ویرایش تخفیف

**Request Body:** مشابه POST

#### `DELETE /api/admin/discounts/{id}`
غیرفعال کردن تخفیف

**Response:** `204 No Content`

#### `GET /api/admin/discounts/{id}/usage-history`
تاریخچه استفاده از تخفیف

**Response:**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174007",
    "discountId": "123e4567-e89b-12d3-a456-426614174006",
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "orderId": "123e4567-e89b-12d3-a456-426614174008",
    "discountAmount": 3000000,
    "cartTotal": 10000000,
    "finalTotal": 7000000,
    "couponCode": "NOWRUZ30",
    "usedAt": "2024-03-22T14:30:00Z",
    "userEmail": "user@example.com",
    "discountName": "تخفیف عید نوروز"
  }
]
```

### 3. عملیات کاربری

#### `GET /api/users/discounts/my-discount-history`
تاریخچه تخفیف‌های کاربر

**Headers:**
```
Authorization: Bearer USER_JWT_TOKEN
```

**Query Parameters:**
- `pageNumber` (int): شماره صفحه
- `pageSize` (int): اندازه صفحه

**Response:** مشابه usage-history

## 📊 انواع تخفیف

### 1. تخفیف درصدی (Percentage)
```json
{
  "type": 1,
  "value": 20,  // 20 درصد
  "maxDiscountAmount": 5000000  // حداکثر مبلغ تخفیف
}
```

### 2. تخفیف مبلغ ثابت (FixedAmount)
```json
{
  "type": 2,
  "value": 500000  // 500,000 تومان
}
```

### 3. خرید X، دریافت Y رایگان (BuyXGetYFree)
```json
{
  "type": 3,
  "buyQuantity": 2,
  "getQuantity": 1,
  "applicability": 2,  // فقط محصولات خاص
  "applicableProductIds": ["product-id"]
}
```

### 4. ارسال رایگان (FreeShipping)
```json
{
  "type": 4,
  "value": 0,  // مقدار اهمیت ندارد
  "minimumCartAmount": 1000000  // حداقل مبلغ سبد
}
```

## 🎯 محدوده اعمال تخفیف

### 1. کل سبد خرید (EntireCart)
```json
{
  "applicability": 1
}
```

### 2. محصولات خاص (SpecificProducts)
```json
{
  "applicability": 2,
  "applicableProductIds": [
    "123e4567-e89b-12d3-a456-426614174001",
    "123e4567-e89b-12d3-a456-426614174002"
  ]
}
```

### 3. دسته‌بندی‌های خاص (SpecificCategories)
```json
{
  "applicability": 3,
  "applicableCategoryIds": [
    "123e4567-e89b-12d3-a456-426614174001"
  ]
}
```

## ⚠️ کدهای خطا

### کدهای HTTP:
- `200` - موفقیت‌آمیز
- `400` - خطا در درخواست
- `401` - احراز هویت نشده
- `403` - عدم دسترسی
- `404` - یافت نشد
- `500` - خطای سرور

### خطاهای رایج:

#### تخفیف منقضی شده:
```json
{
  "isSuccess": false,
  "errorMessage": "کد تخفیف منقضی شده است",
  "discountAmount": 0,
  "finalTotal": 16050000
}
```

#### تخفیف تمام شده:
```json
{
  "isSuccess": false,
  "errorMessage": "ظرفیت استفاده از این کد تخفیف تمام شده است",
  "discountAmount": 0,
  "finalTotal": 16050000
}
```

#### حداقل مبلغ رعایت نشده:
```json
{
  "isSuccess": false,
  "errorMessage": "مبلغ سبد خرید کمتر از حداقل مقدار مورد نیاز است",
  "discountAmount": 0,
  "finalTotal": 16050000
}
```

## 🔍 نمونه‌های کاربردی

### 1. تخفیف 20% برای خرید بالای 1 میلیون:
```bash
curl -X POST http://localhost:8080/api/discounts/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "items": [
      {
        "productId": "laptop-1",
        "productName": "لپ‌تاپ",
        "categoryId": "electronics",
        "categoryName": "الکترونیک",
        "unitPrice": 1200000,
        "quantity": 1
      }
    ],
    "shippingCost": 50000,
    "couponCode": "SAVE20"
  }'
```

### 2. ایجاد کمپین BOGO:
```bash
curl -X POST http://localhost:8080/api/admin/discounts \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "خرید 2 عدد، 1 عدد رایگان",
    "description": "کمپین ویژه کفش ورزشی",
    "type": 3,
    "buyQuantity": 2,
    "getQuantity": 1,
    "startDate": "2024-06-01T00:00:00Z",
    "endDate": "2024-06-30T23:59:59Z",
    "applicability": 3,
    "applicableCategoryIds": ["sports-shoes"]
  }'
```

### 3. تخفیف ارسال رایگان:
```bash
curl -X POST http://localhost:8080/api/admin/discounts \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "ارسال رایگان",
    "description": "ارسال رایگان برای خرید بالای 500 هزار تومان",
    "type": 4,
    "value": 0,
    "minimumCartAmount": 500000,
    "isAutomatic": true,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "applicability": 1
  }'
```

## 🚀 نکات عملکرد

### کش:
- تخفیف‌های خودکار: 5 دقیقه
- کدهای تخفیف: 10 دقیقه
- تخفیف‌های انفرادی: 15 دقیقه

### محدودیت‌ها:
- حداکثر 100 آیتم در سبد
- حداکثر 50 تخفیف همزمان
- حداکثر 10 درخواست در ثانیه هر کاربر

### بهینه‌سازی:
- از pagination استفاده کنید
- فیلترهای جستجو را محدود کنید
- کدهای تخفیف را cache کنید

---

**نکته:** این مستندات برای نسخه v1.0 سرویس تخفیف نوشته شده است.
