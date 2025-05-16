using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

public class Discount
{
    public string DiscountCode { get; set; } // کد تخفیف (اگر مربوط به کد خاصی باشد)
    public decimal Percentage { get; set; } // درصد تخفیف
    public decimal FixedAmount { get; set; } // مقدار ثابت تخفیف (یکی از این دو معمولا استفاده می‌شود)
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? StartDate { get; set; } // تاریخ شروع اعتبار
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? EndDate { get; set; } // تاریخ پایان اعتبار
    public bool IsActive { get; set; } = true;
}