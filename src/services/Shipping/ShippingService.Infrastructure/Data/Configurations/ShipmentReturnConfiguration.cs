using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Enums;

namespace ShippingService.Infrastructure.Data.Configurations;

public class ShipmentReturnConfiguration : IEntityTypeConfiguration<ShipmentReturn>
{
    public void Configure(EntityTypeBuilder<ShipmentReturn> builder)
    {
        builder.ToTable("shipment_returns", "shipping");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.OriginalShipmentId)
            .HasColumnName("original_shipment_id")
            .IsRequired();

        builder.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasColumnName("reason")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.ReasonDescription)
            .HasColumnName("reason_description")
            .HasColumnType("text");

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.ReturnTrackingNumber)
            .HasColumnName("return_tracking_number")
            .HasMaxLength(50);

        builder.Property(r => r.RequestedDate)
            .HasColumnName("requested_date")
            .IsRequired();

        builder.Property(r => r.ApprovedDate)
            .HasColumnName("approved_date");

        builder.Property(r => r.CompletedDate)
            .HasColumnName("completed_date");

        builder.Property(r => r.ApprovedByUserId)
            .HasColumnName("approved_by_user_id")
            .HasMaxLength(100);

        builder.Property(r => r.RefundAmount)
            .HasColumnName("refund_amount")
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.IsRefundProcessed)
            .HasColumnName("is_refund_processed");

        builder.Property(r => r.RefundProcessedDate)
            .HasColumnName("refund_processed_date");

        builder.Property(r => r.RefundTransactionId)
            .HasColumnName("refund_transaction_id")
            .HasMaxLength(200);

        builder.Property(r => r.CollectionAddress)
            .HasColumnName("collection_address")
            .HasMaxLength(500);

        builder.Property(r => r.CollectionDate)
            .HasColumnName("collection_date");

        builder.Property(r => r.CollectionNotes)
            .HasColumnName("collection_notes")
            .HasColumnType("text");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        // Relationships
        builder.HasOne(r => r.OriginalShipment)
            .WithMany()
            .HasForeignKey(r => r.OriginalShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.TrackingHistory)
            .WithOne()
            .HasForeignKey(t => t.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.ReturnTrackingNumber).IsUnique();
        builder.HasIndex(r => r.CustomerId);
        builder.HasIndex(r => r.OriginalShipmentId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.Status, r.RequestedDate });
    }
}

public class ReturnTrackingConfiguration : IEntityTypeConfiguration<ReturnTracking>
{
    public void Configure(EntityTypeBuilder<ReturnTracking> builder)
    {
        builder.ToTable("return_trackings", "shipping");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.ReturnId)
            .HasColumnName("return_id")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();

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
        builder.HasIndex(t => t.ReturnId);
        builder.HasIndex(t => new { t.ReturnId, t.Timestamp });
    }
}
