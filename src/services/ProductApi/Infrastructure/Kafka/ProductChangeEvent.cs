namespace ProductApi.Infrastructure.Kafka
{
    public class ProductChangeEvent
    {
        public string ChangeType { get; set; } // "Created", "Updated", "Deleted"
        public string ProductId { get; set; }
        public string Sku { get; set; } // برای شناسایی بهتر توسط سایر سرویس‌ها
        public DateTime Timestamp { get; set; }
        // public object ProductData { get; set; } // (اختیاری) ارسال داده‌های بیشتر
    }
}
