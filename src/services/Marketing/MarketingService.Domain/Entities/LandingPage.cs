using MarketingService.Domain.Enums;

namespace MarketingService.Domain.Entities;

public class LandingPage
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Slug { get; private set; }
    public string Content { get; private set; }
    public string MetaDescription { get; private set; }
    public string MetaKeywords { get; private set; }
    public LandingPageStatus Status { get; private set; }
    public string? CustomCss { get; private set; }
    public string? CustomJs { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }

    private LandingPage()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Content = string.Empty;
        MetaDescription = string.Empty;
        MetaKeywords = string.Empty;
        CreatedBy = string.Empty;
    }

    public LandingPage(
        string title,
        string slug,
        string content,
        string metaDescription,
        string metaKeywords,
        string createdBy)
    {
        Id = Guid.NewGuid();
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        MetaDescription = metaDescription ?? throw new ArgumentNullException(nameof(metaDescription));
        MetaKeywords = metaKeywords ?? throw new ArgumentNullException(nameof(metaKeywords));
        Status = LandingPageStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
    }

    public void UpdateContent(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(LandingPageStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMetaData(string metaDescription, string metaKeywords)
    {
        MetaDescription = metaDescription ?? throw new ArgumentNullException(nameof(metaDescription));
        MetaKeywords = metaKeywords ?? throw new ArgumentNullException(nameof(metaKeywords));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCustomStyling(string? customCss, string? customJs)
    {
        CustomCss = customCss;
        CustomJs = customJs;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPublished => Status == LandingPageStatus.Published;
}
