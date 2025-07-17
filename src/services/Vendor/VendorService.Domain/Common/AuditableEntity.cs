namespace VendorService.Domain.Common;


public abstract class AuditableEntity : BaseEntity
{
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
     public bool IsDeleted { get; set; }
     public bool IsPublished { get; set; } = false;
}