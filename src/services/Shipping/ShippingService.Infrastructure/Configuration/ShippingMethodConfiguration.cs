using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingService.Domain.Entities;
using ShippingService.Domain.ValueObjects;
using System.Text.Json;

namespace ShippingService.Infrastructure.Configuration;

public class ShippingMethodConfiguration : IEntityTypeConfiguration<ShippingMethod>
{
    public void Configure(EntityTypeBuilder<ShippingMethod> builder)
    {
        builder.ToTable("shipping_methods");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(x => x.BaseCost)
            .HasColumnName("base_cost")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(x => x.RequiresTimeSlot)
            .HasColumnName("requires_time_slot")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Configure CostRules as JSON
        builder.OwnsMany(x => x.CostRules, costRule =>
        {
            costRule.ToJson("cost_rules");
            costRule.Property(r => r.RuleType).HasConversion<int>();
        });

        // Configure RestrictionRules as JSON
        builder.OwnsMany(x => x.RestrictionRules, restrictionRule =>
        {
            restrictionRule.ToJson("restriction_rules");
            restrictionRule.Property(r => r.RuleType).HasConversion<int>();
        });

        // Configure relationships
        builder.HasMany(x => x.TimeSlotTemplates)
            .WithOne(x => x.ShippingMethod)
            .HasForeignKey(x => x.ShippingMethodId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.IsActive);
    }
}
