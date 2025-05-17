using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

public class Discount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // شناسه یکتا

    public string DiscountCode { get; set; } // کد تخفیف (اگر مربوط به کد خاصی باشد)
    public decimal Percentage { get; set; } // درصد تخفیف
    public decimal FixedAmount { get; set; } // مقدار ثابت تخفیف (یکی از این دو معمولا استفاده می‌شود)
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? StartDate { get; set; } // تاریخ شروع اعتبار
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? EndDate { get; set; } // تاریخ پایان اعتبار
    public bool IsActive { get; set; } = true;

    public string Description { get; set; } // توضیحات تخفیف

    public int UsageLimit { get; set; } // حداکثر تعداد دفعات استفاده
    public int UsedCount { get; set; } // تعداد دفعات استفاده شده

    public List<string> ApplicableProductIds { get; set; } // لیست محصولات قابل استفاده

    public bool IsValid()
    {
        var now = DateTime.UtcNow;
        bool dateValid = (!StartDate.HasValue || StartDate.Value <= now) &&
                         (!EndDate.HasValue || EndDate.Value >= now);
        bool usageValid = UsageLimit == 0 || UsedCount < UsageLimit;
        return IsActive && dateValid && usageValid;
    }
}
