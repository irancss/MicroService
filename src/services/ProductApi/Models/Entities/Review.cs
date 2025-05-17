using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

[BsonIgnoreExtraElements]
public class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string ProductId { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }

    [BsonElement("rating")]
    public int Rating { get; set; }

    [BsonElement("comment")]
    public string Comment { get; set; }

    [BsonElement("isApproved")]
    public bool IsApproved { get; set; } = false;

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("adminReply")]
    public string? AdminReply { get; set; }

    public void Approve(string? adminReply = null)
    {
        IsApproved = true;
        AdminReply = adminReply;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(int rating, string comment)
    {
        Rating = rating;
        Comment = comment;
        UpdatedAt = DateTime.UtcNow;
    }
}
