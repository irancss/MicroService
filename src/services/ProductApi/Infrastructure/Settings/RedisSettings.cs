namespace ProductApi.Infrastructure.Settings
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = null!;
        public bool AbortOnConnectFail { get; set; } = false; // تنظیم پیش‌فرض برای جلوگیری از خطا در استارت آپ
    }
}
