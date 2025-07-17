using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Enums;

namespace ShippingService.Infrastructure.Data.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("shipments", "shipping");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.ShippingMethodId)
            .HasColumnName("shipping_method_id")
            .IsRequired();

        builder.Property(s => s.OriginAddress)
            .HasColumnName("origin_address")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.DestinationAddress)
            .HasColumnName("destination_address")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.OriginCity)
            .HasColumnName("origin_city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.DestinationCity)
            .HasColumnName("destination_city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.OriginLatitude)
            .HasColumnName("origin_latitude")
            .HasColumnType("double precision");

        builder.Property(s => s.OriginLongitude)
            .HasColumnName("origin_longitude")
            .HasColumnType("double precision");

        builder.Property(s => s.DestinationLatitude)
            .HasColumnName("destination_latitude")
            .HasColumnType("double precision");

        builder.Property(s => s.DestinationLongitude)
            .HasColumnName("destination_longitude")
            .HasColumnType("double precision");

        builder.Property(s => s.Weight)
            .HasColumnName("weight")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Width)
            .HasColumnName("width")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Height)
            .HasColumnName("height")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.Length)
            .HasColumnName("length")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.DeclaredValue)
            .HasColumnName("declared_value")
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.TotalCost)
            .HasColumnName("total_cost")
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(s => s.EstimatedDeliveryDate)
            .HasColumnName("estimated_delivery_date")
            .IsRequired();

        builder.Property(s => s.ActualDeliveryDate)
            .HasColumnName("actual_delivery_date");

        builder.Property(s => s.TrackingNumber)
            .HasColumnName("tracking_number")
            .HasMaxLength(50);

        builder.Property(s => s.OptimizedRoute)
            .HasColumnName("optimized_route")
            .HasColumnType("text");

        builder.Property(s => s.EstimatedDistance)
            .HasColumnName("estimated_distance")
            .HasColumnType("decimal(10,2)");

        builder.Property(s => s.EstimatedDuration)
            .HasColumnName("estimated_duration");

        builder.Property(s => s.DeliveryDriverId)
            .HasColumnName("delivery_driver_id")
            .HasMaxLength(100);

        builder.Property(s => s.DeliveryDriverName)
            .HasColumnName("delivery_driver_name")
            .HasMaxLength(200);

        builder.Property(s => s.DeliveryDriverPhone)
            .HasColumnName("delivery_driver_phone")
            .HasMaxLength(20);

        builder.Property(s => s.DeliveryNotes)
            .HasColumnName("delivery_notes")
            .HasColumnType("text");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        // Relationships
        builder.HasOne(s => s.ShippingMethod)
            .WithMany()
            .HasForeignKey(s => s.ShippingMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.TrackingHistory)
            .WithOne()
            .HasForeignKey(t => t.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.TrackingNumber).IsUnique();
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.OrderId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.DeliveryDriverId);
        builder.HasIndex(s => new { s.Status, s.EstimatedDeliveryDate });
    }
}

public class ShipmentTrackingConfiguration : IEntityTypeConfiguration<ShipmentTracking>
{
    public void Configure(EntityTypeBuilder<ShipmentTracking> builder)
    {
        builder.ToTable("shipment_trackings", "shipping");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.ShipmentId)
            .HasColumnName("shipment_id")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

        builder.Property(t => t.Location)
            .HasColumnName("location")
            .HasMaxLength(500);

        builder.Property(t => t.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        builder.Property(t => t.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .HasMaxLength(100);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .HasColumnName("updated_at");

        // Indexes
        builder.HasIndex(t => t.ShipmentId);
        builder.HasIndex(t => new { t.ShipmentId, t.Timestamp });
    }
}
