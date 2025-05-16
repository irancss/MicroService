using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

[BsonIgnoreExtraElements]
public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string ProductId { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; } // شناسه کاربر پرسش کننده

    [BsonElement("questionText")]
    public string QuestionText { get; set; }

    [BsonElement("answers")]
    public List<Answer> Answers { get; set; } = new List<Answer>();

    [BsonElement("isAnswered")]
    public bool IsAnswered => Answers.Any(); // فیلد محاسبه شده (ممکن است در MongoDB ذخیره نشود)

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}