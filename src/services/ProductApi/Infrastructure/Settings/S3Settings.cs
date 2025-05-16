namespace ProductApi.Infrastructure.Settings
{
    public class S3Settings
    {
        public string ServiceUrl { get; set; } // برای سازگاری با MinIO و ... (اختیاری)
        public string BucketName { get; set; } = null!;
        public string Region { get; set; } = null!; // مانند "us-east-1"
        public string? AccessKey { get; set; } // در محیط عملیاتی از IAM Role استفاده شود
        public string? SecretKey { get; set; } // در محیط عملیاتی از IAM Role استفاده شود
        public int PresignedUrlExpirationMinutes { get; set; } = 15; // زمان اعتبار لینک آپلود
    }
}
