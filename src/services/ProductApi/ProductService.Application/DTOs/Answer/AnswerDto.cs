namespace ProductService.Application.DTOs.Answer
{
    public class AnswerDto 
    {
        public Guid Id { get; set; }
        public string QuestionId { get; set; } 
        public string UserId { get; set; } 
        public string AnswerText { get; set; }
        public bool IsAdminAnswer { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CreateAnswerDto
    {
        public string QuestionId { get; set; }
        public string UserId { get; set; }
        public string AnswerText { get; set; }
        public bool IsAdminAnswer { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }

    public class UpdateAnswerDto
    {
        public string Id { get; set; }
        public string QuestionId { get; set; }
        public string UserId { get; set; }
        public string AnswerText { get; set; }
        public bool IsAdminAnswer { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }
}
