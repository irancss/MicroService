
using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Configurations;

public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable(typeof(T).Name + "s"); // Pluralize the table name based on the entity type
        builder.Property(e => e.Id).ValueGeneratedOnAdd(); // Assuming Id is a Guid or int, adjust as necessary
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}