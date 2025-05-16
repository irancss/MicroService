namespace ProductApi.Infrastructure.Settings
{
    public class MinioSettings
    {
        public string ServiceUrl { get; set; } = null!; // آدرس MinIO شما (مثلا "http://localhost:9000")
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string BucketName { get; set; } = null!;
        public int PresignedUrlExpirationMinutes { get; set; } = 15;
        public bool UseHttps { get; set; } = false; // اگر MinIO با HTTPS اجرا می‌شود true کنید
        public string PendingUploadPrefix { get; set; } = "pending-uploads/"; // پیشوند برای فایل‌های موقت

    }
}
