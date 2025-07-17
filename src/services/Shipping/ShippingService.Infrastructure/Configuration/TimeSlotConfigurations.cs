using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;

namespace ShippingService.Infrastructure.Configuration;

public class TimeSlotTemplateConfiguration : IEntityTypeConfiguration<TimeSlotTemplate>
{
    public void Configure(EntityTypeBuilder<TimeSlotTemplate> builder)
    {
        builder.ToTable("time_slot_templates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.ShippingMethodId)
            .HasColumnName("shipping_method_id")
            .IsRequired();

        builder.Property(x => x.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(x => x.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // Relationships
        builder.HasOne(x => x.ShippingMethod)
            .WithMany(x => x.TimeSlotTemplates)
            .HasForeignKey(x => x.ShippingMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Bookings)
            .WithOne(x => x.TimeSlotTemplate)
            .HasForeignKey(x => x.TimeSlotTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => new { x.ShippingMethodId, x.DayOfWeek });
        builder.HasIndex(x => x.IsActive);
    }
}

public class TimeSlotBookingConfiguration : IEntityTypeConfiguration<TimeSlotBooking>
{
    public void Configure(EntityTypeBuilder<TimeSlotBooking> builder)
    {
        builder.ToTable("time_slot_bookings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.TimeSlotTemplateId)
            .HasColumnName("time_slot_template_id")
            .IsRequired();

        builder.Property(x => x.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        // Relationships
        builder.HasOne(x => x.TimeSlotTemplate)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.TimeSlotTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => new { x.TimeSlotTemplateId, x.Date });
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.IsActive);
    }
}
