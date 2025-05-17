namespace OrderApi.Enums
{
    public enum OrderStatus
    {
        Pending,        // در انتظار بررسی
        Confirmed,      // تایید شده
        Cancelled,      // لغو شده
        Shipped,        // ارسال شده
        Delivered,      // تحویل داده شده
        Returned,       // مرجوع شده
        Processing,     // در حال پردازش
        OnHold,         // در انتظار
        Failed,         // ناموفق
        Refunded        // بازپرداخت شده
    }
}
