namespace OrderService.Core.Enums;

public enum PaymentStatus
{
    Pending,        // در انتظار پرداخت
    Paid,           // پرداخت شده
    Failed,         // پرداخت ناموفق
    Refunded,       // بازپرداخت شده
    Cancelled       // لغو شده
}