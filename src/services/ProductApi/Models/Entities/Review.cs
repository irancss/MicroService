using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

[BsonIgnoreExtraElements]
public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)] // یا اگر از Product.Id استفاده می‌کنید، نوع آن را ObjectId بگذارید
    [BsonElement("productId")]
    public string ProductId { get; set; } // شناسه محصول مرتبط

    [BsonElement("userId")]
    public string UserId { get; set; } // شناسه کاربر نظر دهنده

    [BsonElement("rating")]
    public int Rating { get; set; } // امتیاز (مثلا 1 تا 5)

    [BsonElement("comment")]
    public string Comment { get; set; }

    [BsonElement("isApproved")]
    public bool IsApproved { get; set; } = false; // نیاز به تایید مدیر

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}