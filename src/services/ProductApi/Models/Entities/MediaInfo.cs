namespace ProductApi.Models.Entities
{
    public class MediaInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // شناسه یکتا برای هر مدیا
        public string Url { get; set; } // URL ثابت: http://your-host:9000/bucket/key
        public string S3Key { get; set; } // کلید فایل در S3 (برای مدیریت داخلی)
        public string MediaType { get; set; } // "Image", "Video"
        public string AltText { get; set; } // متن جایگزین برای تصاویر
        public int Order { get; set; } // ترتیب نمایش
    }
}
