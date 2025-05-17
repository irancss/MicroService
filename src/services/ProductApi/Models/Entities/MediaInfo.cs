namespace ProductApi.Models.Entities
{
    public class MediaInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // شناسه یکتا برای هر مدیا
        public string Url { get; set; } // URL ثابت: http://your-host:9000/bucket/key
        public string MediaType { get; set; } // "Image", "Video"
        public string AltText { get; set; } // متن جایگزین برای تصاویر
        public int Order { get; set; } // ترتیب نمایش

        // تاریخ ایجاد مدیا
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // تاریخ آخرین ویرایش
        public DateTime? UpdatedAt { get; set; }

        // وضعیت فعال یا غیرفعال بودن مدیا
        public bool IsActive { get; set; } = true;

        // توضیحات اختیاری برای مدیا
        public string? Description { get; set; }

        // متد برای به‌روزرسانی اطلاعات مدیا
        public void Update(string url, string mediaType, string altText, int order, string? description = null)
        {
            Url = url;
            MediaType = mediaType;
            AltText = altText;
            Order = order;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        // متد غیرفعال‌سازی مدیا
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // متد فعال‌سازی مدیا
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
