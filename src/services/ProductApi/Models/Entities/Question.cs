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
    public bool IsAnswered => Answers != null && Answers.Any(); // فیلد محاسبه شده (ممکن است در MongoDB ذخیره نشود)

    [BsonElement("createdAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    public void AddAnswer(Answer answer)
    {
        if (Answers == null)
            Answers = new List<Answer>();
        Answers.Add(answer);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAnswer(string answerId)
    {
        if (Answers == null)
            return;
        var answer = Answers.FirstOrDefault(a => a.Id == answerId);
        if (answer != null)
        {
            Answers.Remove(answer);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
