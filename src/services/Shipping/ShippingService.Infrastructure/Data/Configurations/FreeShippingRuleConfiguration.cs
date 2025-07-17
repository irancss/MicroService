using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations;

/// <summary>
/// تنظیمات Entity Framework برای قوانین ارسال رایگان
/// </summary>
public class FreeShippingRuleConfiguration : IEntityTypeConfiguration<FreeShippingRule>
{
    /// <summary>
    /// تنظیم کردن مدل قانون ارسال رایگان
    /// </summary>
    /// <param name="builder">سازنده مدل</param>
    public void Configure(EntityTypeBuilder<FreeShippingRule> builder)
    {
        // تنظیم نام جدول
        builder.ToTable("free_shipping_rules");

        // تنظیم کلید اصلی
        builder.HasKey(x => x.Id);

        // تنظیم فیلدها
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.Priority)
            .HasColumnName("priority")
            .IsRequired();

        builder.Property(x => x.DiscountType)
            .HasColumnName("discount_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DiscountValue)
            .HasColumnName("discount_value")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.MaxDiscountAmount)
            .HasColumnName("max_discount_amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date");

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date");

        builder.Property(x => x.MaxUsageCount)
            .HasColumnName("max_usage_count");

        builder.Property(x => x.CurrentUsageCount)
            .HasColumnName("current_usage_count")
            .IsRequired();

        builder.Property(x => x.IsPremiumOnly)
            .HasColumnName("is_premium_only")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // تنظیم روابط
        builder.HasMany(x => x.Conditions)
            .WithOne(x => x.Rule)
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Cascade);

        // تنظیم ایندکس‌ها
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("ix_free_shipping_rules_is_active");

        builder.HasIndex(x => x.Priority)
            .HasDatabaseName("ix_free_shipping_rules_priority");

        builder.HasIndex(x => new { x.IsActive, x.Priority })
            .HasDatabaseName("ix_free_shipping_rules_active_priority");

        builder.HasIndex(x => x.StartDate)
            .HasDatabaseName("ix_free_shipping_rules_start_date");

        builder.HasIndex(x => x.EndDate)
            .HasDatabaseName("ix_free_shipping_rules_end_date");
    }
}
