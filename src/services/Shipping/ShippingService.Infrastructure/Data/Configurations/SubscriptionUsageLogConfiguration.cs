using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations;

/// <summary>
/// تنظیمات Entity Framework برای لاگ‌های استفاده از اشتراک
/// </summary>
public class SubscriptionUsageLogConfiguration : IEntityTypeConfiguration<SubscriptionUsageLog>
{
    /// <summary>
    /// تنظیم کردن مدل لاگ استفاده
    /// </summary>
    /// <param name="builder">سازنده مدل</param>
    public void Configure(EntityTypeBuilder<SubscriptionUsageLog> builder)
    {
        // تنظیم نام جدول
        builder.ToTable("subscription_usage_logs");

        // تنظیم کلید اصلی
        builder.HasKey(x => x.Id);

        // تنظیم فیلدها
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.SubscriptionId)
            .HasColumnName("subscription_id")
            .IsRequired();

        builder.Property(x => x.ShipmentId)
            .HasColumnName("shipment_id")
            .IsRequired();

        builder.Property(x => x.SavedAmount)
            .HasColumnName("saved_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.UsageDate)
            .HasColumnName("usage_date")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // تنظیم روابط
        builder.HasOne(x => x.Subscription)
            .WithMany(x => x.UsageLogs)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // تنظیم ایندکس‌ها
        builder.HasIndex(x => x.SubscriptionId)
            .HasDatabaseName("ix_subscription_usage_logs_subscription_id");

        builder.HasIndex(x => x.ShipmentId)
            .HasDatabaseName("ix_subscription_usage_logs_shipment_id");

        builder.HasIndex(x => x.UsageDate)
            .HasDatabaseName("ix_subscription_usage_logs_usage_date");
    }
}
