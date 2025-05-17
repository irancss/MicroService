using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

public class Answer
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } // شناسه کاربر پاسخ دهنده
    public string AnswerText { get; set; }
    public bool IsAdminAnswer { get; set; } = false;
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
