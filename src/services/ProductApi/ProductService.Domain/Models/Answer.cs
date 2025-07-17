using ProductService.Domain.Common;

public class Answer : AuditableEntity
{

    public string QuestionId { get; set; } // شناسه سوالی که این پاسخ به آن تعلق دارد

    public string UserId { get; set; } // شناسه کاربر پاسخ دهنده

    public string AnswerText { get; set; }

    public bool IsAdminAnswer { get; set; }


    public bool IsApproved { get; set; } = false; // Indicates if the answer is approved by an admin
    public Question Question { get; set; }


    // Constructor to initialize required properties and set defaults
    public Answer(string questionId, string userId, string answerText, bool isAdminAnswer = false)
    {
        Id = Guid.NewGuid().ToString();
        QuestionId = questionId ?? throw new ArgumentNullException(nameof(questionId));
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        AnswerText = answerText ?? throw new ArgumentNullException(nameof(answerText));
        IsAdminAnswer = isAdminAnswer;
        CreatedAt = DateTime.UtcNow;
    }

    // Parameterless constructor for frameworks like EF Core
    private Answer()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        // Required properties will be set by the ORM or through property initializers
        QuestionId = string.Empty; // Or null if your ORM handles it
        UserId = string.Empty;     // Or null
        AnswerText = string.Empty; // Or null
    }
}
