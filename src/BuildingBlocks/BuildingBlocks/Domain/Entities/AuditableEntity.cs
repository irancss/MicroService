namespace BuildingBlocks.Domain.Entities;

/// <summary>
/// An abstract base class for entities that require auditing.
/// </summary>
public abstract class AuditableEntity<TId> : BaseEntity<TId> where TId : notnull
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>
/// A convenient base class for auditable entities with a Guid ID.
/// </summary>
public abstract class AuditableEntity : AuditableEntity<string>
{
}