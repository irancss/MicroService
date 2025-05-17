using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

[BsonIgnoreExtraElements]
public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("sku")]
    public string Sku { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("categories")]
    public List<string> Categories { get; set; } = new List<string>();

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    [BsonElement("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

    [BsonElement("media")]
    public List<MediaInfo> Media { get; set; } = new List<MediaInfo>();

    [BsonElement("averageRating")]
    public double AverageRating { get; set; }

    [BsonElement("reviewCount")]
    public int ReviewCount { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // New fields for completeness

    [BsonElement("stockQuantity")]
    public int StockQuantity { get; set; } = 0; // موجودی انبار

    [BsonElement("brand")]
    public string Brand { get; set; } // برند محصول

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new List<string>(); // برچسب‌ها

    [BsonElement("weight")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Weight { get; set; } // وزن محصول (اختیاری)

    [BsonElement("dimensions")]
    public ProductDimensions Dimensions { get; set; } // ابعاد محصول (اختیاری)

    [BsonElement("vendorId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string VendorId { get; set; } // شناسه فروشنده

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false; // حذف منطقی
    [BsonElement("scheduledDiscounts")]
    public List<ScheduledDiscount> ScheduledDiscounts { get; set; } = new List<ScheduledDiscount>();


}