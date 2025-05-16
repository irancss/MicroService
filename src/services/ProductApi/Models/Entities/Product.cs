using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId] // شناسه اصلی سند در MongoDB
    [BsonRepresentation(BsonType.ObjectId)] // نوع ObjectId برای شناسه
    public string Id { get; set; }

    [BsonElement("sku")] // نام فیلد در MongoDB (اختیاری، پیش‌فرض نام پراپرتی است)
    public string Sku { get; set; } // شناسه یکتای محصول (Stock Keeping Unit)

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("categories")]
    public List<string> Categories { get; set; } = new List<string>(); // لیست شناسه‌ها یا نام‌های دسته‌بندی

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)] // دقت بالا برای قیمت
    public decimal Price { get; set; }

    // برای ویژگی‌های متغیر و پویا، Dictionary یا BsonDocument مناسب است
    [BsonElement("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    // مثال: { "Color": "Red", "Size": "XL", "Material": "Cotton" }

    [BsonElement("media")]
    public List<MediaInfo> Media { get; set; } = new List<MediaInfo>();


    [BsonElement("averageRating")]
    public double AverageRating { get; set; } // محاسبه شده بر اساس نظرات

    [BsonElement("reviewCount")]
    public int ReviewCount { get; set; } // تعداد نظرات تایید شده

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}