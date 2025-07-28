using BuildingBlocks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductService.Infrastructure.Domain;

public abstract class BaseEntityTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditableEntity<Guid>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        // نکته: اگر Id را در کد C# تولید می‌کنی (مثلاً با Guid.NewGuid()) بهتر است خط زیر را حذف کنی
        // چون ValueGeneratedOnAdd بیشتر برای Id های عددی (int, long) یا Guid های دیتابیسی (newsequentialid) است.
        // builder.Property(e => e.Id).ValueGeneratedOnAdd(); 

        builder.ToTable(typeof(T).Name + "s");
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
