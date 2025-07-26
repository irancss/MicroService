

using ProductService.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using BuildingBlocks.Domain.Entities;

public class Question : AuditableEntity
{
    // شناسه سوال، به طور پیش فرض با یک GUID جدید مقداردهی اولیه می شود

    public string ProductId { get; set; }

    public string UserId { get; set; } // شناسه کاربر پرسش کننده

    public string QuestionText { get; set; }

    // لیست پاسخ ها، با setter خصوصی برای جلوگیری از جایگزینی لیست از خارج
    public List<Answer> Answers { get; private set; } = new List<Answer>();

    // فیلد محاسبه شده برای بررسی اینکه آیا سوال پاسخی دارد یا خیر
    // با توجه به اینکه Answers هرگز null نخواهد بود (به دلیل مقداردهی اولیه و setter خصوصی)، فقط Count > 0 کافی است
    [NotMapped] // این خصوصیت به EF Core (یا مشابه) می گوید که این فیلد را در پایگاه داده ذخیره نکند
    public bool IsAnswered => Answers.Count > 0; 

    public bool IsApproved { get; set; } = false;

    public Product Product { get; set; }

    public bool IsActive { get; set; } = true;

    // سازنده پیش فرض برای استفاده توسط فریم ورک ها (مانند سریالایزرها یا ORM ها)
    // مقداردهی اولیه خصوصیات توسط property initializers انجام می شود
    public Question()
    {
    }

    // سازنده برای ایجاد یک سوال جدید با اطلاعات ضروری
    public Question(string productId, string userId, string questionText) : this()
    {
        // Id, Answers, CreatedAt, IsActive توسط property initializers مقداردهی اولیه شده اند
        this.ProductId = productId;
        this.UserId = userId;
        this.QuestionText = questionText;
    }

    public void AddAnswer(Answer answer)
    {
        // لیست Answers به دلیل مقداردهی اولیه و setter خصوصی، تضمین شده است که null نباشد
        this.Answers.Add(answer);
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAnswer(string answerId)
    {
        // لیست Answers تضمین شده است که null نباشد
        var answer = this.Answers.FirstOrDefault(a => a.Id == answerId);
        if (answer != null)
        {
            this.Answers.Remove(answer);
            this.UpdatedAt = DateTime.UtcNow;
        }
    }

    // متد برای غیرفعال کردن سوال
    public void Deactivate()
    {
        this.IsActive = false;
        this.UpdatedAt = DateTime.UtcNow;
    }

    // متد برای فعال کردن سوال
    public void Activate()
    {
        this.IsActive = true;
        this.UpdatedAt = DateTime.UtcNow;
    }
}
