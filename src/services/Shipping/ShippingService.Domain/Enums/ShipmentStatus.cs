namespace ShippingService.Domain.Enums;

/// <summary>
/// وضعیت مرسوله
/// </summary>
public enum ShipmentStatus
{
    /// <summary>
    /// ایجاد شده
    /// </summary>
    Created = 1,
    
    /// <summary>
    /// در انتظار جمع‌آوری
    /// </summary>
    PickupScheduled = 2,
    
    /// <summary>
    /// جمع‌آوری شده
    /// </summary>
    PickedUp = 3,
    
    /// <summary>
    /// در حال انتقال
    /// </summary>
    InTransit = 4,
    
    /// <summary>
    /// در حال تحویل
    /// </summary>
    OutForDelivery = 5,
    
    /// <summary>
    /// تحویل داده شده
    /// </summary>
    Delivered = 6,
    
    /// <summary>
    /// تحویل ناموفق
    /// </summary>
    Failed = 7,
    
    /// <summary>
    /// لغو شده
    /// </summary>
    Cancelled = 8,
    
    /// <summary>
    /// مرجوع شده
    /// </summary>
    Returned = 9
}

/// <summary>
/// وضعیت اشتراک ویژه
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// فعال
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// تعلیق شده
    /// </summary>
    Suspended = 2,
    
    /// <summary>
    /// لغو شده
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// منقضی شده
    /// </summary>
    Expired = 4
}

/// <summary>
/// نوع تخفیف
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// درصدی
    /// </summary>
    Percentage = 1,
    
    /// <summary>
    /// مبلغ ثابت
    /// </summary>
    FixedAmount = 2,
    
    /// <summary>
    /// ارسال رایگان
    /// </summary>
    FreeShipping = 3
}

/// <summary>
/// نوع شرط
/// </summary>
public enum ConditionType
{
    /// <summary>
    /// مبلغ سفارش
    /// </summary>
    OrderAmount = 1,
    
    /// <summary>
    /// تعداد آیتم
    /// </summary>
    ItemCount = 2,
    
    /// <summary>
    /// وزن کل
    /// </summary>
    TotalWeight = 3,
    
    /// <summary>
    /// دسته‌بندی محصول
    /// </summary>
    ProductCategory = 4,
    
    /// <summary>
    /// نوع ارسال
    /// </summary>
    ShippingMethod = 5,
    
    /// <summary>
    /// کد پستی مقصد
    /// </summary>
    DestinationPostalCode = 6,
    
    /// <summary>
    /// شهر مقصد
    /// </summary>
    DestinationCity = 7,
    
    /// <summary>
    /// روز هفته
    /// </summary>
    DayOfWeek = 8,
    
    /// <summary>
    /// کاربر خاص
    /// </summary>
    UserId = 9,
    
    /// <summary>
    /// سطح کاربر
    /// </summary>
    UserLevel = 10
}

/// <summary>
/// عملگر مقایسه
/// </summary>
public enum ComparisonOperator
{
    /// <summary>
    /// مساوی
    /// </summary>
    Equals = 1,
    
    /// <summary>
    /// نامساوی
    /// </summary>
    NotEquals = 2,
    
    /// <summary>
    /// بزرگتر از
    /// </summary>
    GreaterThan = 3,
    
    /// <summary>
    /// بزرگتر یا مساوی
    /// </summary>
    GreaterThanOrEqual = 4,
    
    /// <summary>
    /// کمتر از
    /// </summary>
    LessThan = 5,
    
    /// <summary>
    /// کمتر یا مساوی
    /// </summary>
    LessThanOrEqual = 6,
    
    /// <summary>
    /// شامل
    /// </summary>
    Contains = 7,
    
    /// <summary>
    /// شروع با
    /// </summary>
    StartsWith = 8,
    
    /// <summary>
    /// پایان با
    /// </summary>
    EndsWith = 9,
    
    /// <summary>
    /// در لیست
    /// </summary>
    In = 10
}

/// <summary>
/// نوع داده مقدار
/// </summary>
public enum ValueType
{
    /// <summary>
    /// رشته
    /// </summary>
    String = 1,
    
    /// <summary>
    /// عدد صحیح
    /// </summary>
    Number = 2,
    
    /// <summary>
    /// اعشاری
    /// </summary>
    Decimal = 3,
    
    /// <summary>
    /// بولی
    /// </summary>
    Boolean = 4,
    
    /// <summary>
    /// تاریخ و زمان
    /// </summary>
    DateTime = 5
}

/// <summary>
/// دلایل مرجوعی
/// </summary>
public enum ReturnReason
{
    /// <summary>
    /// کالا آسیب دیده
    /// </summary>
    Damaged = 1,
    
    /// <summary>
    /// کالای اشتباه ارسال شده
    /// </summary>
    WrongItem = 2,
    
    /// <summary>
    /// کالا مطابق توضیحات نیست
    /// </summary>
    NotAsDescribed = 3,
    
    /// <summary>
    /// مشتری نظر عوض کرده
    /// </summary>
    CustomerChanged = 4,
    
    /// <summary>
    /// مسائل کیفیت
    /// </summary>
    QualityIssue = 5,
    
    /// <summary>
    /// تاخیر در تحویل
    /// </summary>
    DeliveryDelay = 6,
    
    /// <summary>
    /// عدم رضایت از محصول
    /// </summary>
    NotSatisfied = 7,
    
    /// <summary>
    /// محصول ناقص
    /// </summary>
    Incomplete = 8,
    
    /// <summary>
    /// سایر دلایل
    /// </summary>
    Other = 99
}

/// <summary>
/// وضعیت مرجوعی
/// </summary>
public enum ReturnStatus
{
    /// <summary>
    /// درخواست مرجوعی ثبت شده
    /// </summary>
    Requested = 1,
    
    /// <summary>
    /// در حال بررسی
    /// </summary>
    UnderReview = 2,
    
    /// <summary>
    /// تایید شده
    /// </summary>
    Approved = 3,
    
    /// <summary>
    /// رد شده
    /// </summary>
    Rejected = 4,
    
    /// <summary>
    /// کالا جمع‌آوری شده
    /// </summary>
    PickedUp = 5,
    
    /// <summary>
    /// در حال انتقال
    /// </summary>
    InTransit = 6,
    
    /// <summary>
    /// دریافت شده در انبار
    /// </summary>
    Received = 7,
    
    /// <summary>
    /// بررسی شده
    /// </summary>
    Inspected = 8,
    
    /// <summary>
    /// تکمیل شده
    /// </summary>
    Completed = 9,
    
    /// <summary>
    /// لغو شده
    /// </summary>
    Cancelled = 10
}

/// <summary>
/// نوع اشتراک ویژه
/// </summary>
public enum SubscriptionType
{
    /// <summary>
    /// اشتراک پایه
    /// </summary>
    Basic = 1,
    
    /// <summary>
    /// اشتراک ویژه
    /// </summary>
    Premium = 2,
    
    /// <summary>
    /// اشتراک طلایی
    /// </summary>
    Gold = 3,
    
    /// <summary>
    /// اشتراک شرکتی
    /// </summary>
    Enterprise = 4
}