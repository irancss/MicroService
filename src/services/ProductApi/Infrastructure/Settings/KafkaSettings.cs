namespace ProductApi.Infrastructure.Settings
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = null!;
        public string ProductChangeTopic { get; set; } = "products.cud"; // تاپیک تغییرات محصول
        // سایر تاپیک‌ها مانند "reviews.cud", "qna.cud"
    }
}
