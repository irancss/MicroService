using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Data.Configurations;

/// <summary>
/// تنظیمات Entity Framework برای اشتراک‌های ویژه
/// </summary>
public class PremiumSubscriptionConfiguration : IEntityTypeConfiguration<PremiumSubscription>
{
    /// <summary>
    /// تنظیم کردن مدل اشتراک ویژه
    /// </summary>
    /// <param name="builder">سازنده مدل</param>
    public void Configure(EntityTypeBuilder<PremiumSubscription> builder)
    {
        // تنظیم نام جدول
        builder.ToTable("premium_subscriptions");

        // تنظیم کلید اصلی
        builder.HasKey(x => x.Id);

        // تنظیم فیلدها
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RemainingFreeRequests)
            .HasColumnName("remaining_free_requests")
            .IsRequired();

        builder.Property(x => x.MaxFreeRequestsPerMonth)
            .HasColumnName("max_free_requests_per_month")
            .IsRequired();

        builder.Property(x => x.LastResetDate)
            .HasColumnName("last_reset_date")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.DurationInDays)
            .HasColumnName("duration_in_days")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // تنظیم روابط
        builder.HasMany(x => x.UsageLogs)
            .WithOne(x => x.Subscription)
            .HasForeignKey(x => x.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        // تنظیم ایندکس‌ها
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_premium_subscriptions_user_id");

        builder.HasIndex(x => new { x.UserId, x.IsActive })
            .HasDatabaseName("ix_premium_subscriptions_user_active");

        builder.HasIndex(x => x.EndDate)
            .HasDatabaseName("ix_premium_subscriptions_end_date");
    }
}
