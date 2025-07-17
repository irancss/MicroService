using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations;

/// <summary>
/// تنظیمات Entity Framework برای شرایط قوانین ارسال رایگان
/// </summary>
public class FreeShippingConditionConfiguration : IEntityTypeConfiguration<FreeShippingCondition>
{
    /// <summary>
    /// تنظیم کردن مدل شرط قانون ارسال رایگان
    /// </summary>
    /// <param name="builder">سازنده مدل</param>
    public void Configure(EntityTypeBuilder<FreeShippingCondition> builder)
    {
        // تنظیم نام جدول
        builder.ToTable("free_shipping_conditions");

        // تنظیم کلید اصلی
        builder.HasKey(x => x.Id);

        // تنظیم فیلدها
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.RuleId)
            .HasColumnName("rule_id")
            .IsRequired();

        builder.Property(x => x.ConditionType)
            .HasColumnName("condition_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.FieldName)
            .HasColumnName("field_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Operator)
            .HasColumnName("operator")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Value)
            .HasColumnName("value")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ValueType)
            .HasColumnName("value_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .HasColumnName("is_required")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // تنظیم روابط
        builder.HasOne(x => x.Rule)
            .WithMany(x => x.Conditions)
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // تنظیم ایندکس‌ها
        builder.HasIndex(x => x.RuleId)
            .HasDatabaseName("ix_free_shipping_conditions_rule_id");

        builder.HasIndex(x => x.ConditionType)
            .HasDatabaseName("ix_free_shipping_conditions_condition_type");

        builder.HasIndex(x => x.FieldName)
            .HasDatabaseName("ix_free_shipping_conditions_field_name");
    }
}
