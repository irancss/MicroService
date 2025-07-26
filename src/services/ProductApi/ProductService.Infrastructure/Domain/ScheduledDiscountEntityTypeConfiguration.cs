using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProductService.Infrastructure.Domain;

public class ScheduledDiscountEntityTypeConfiguration : BaseEntityTypeConfiguration<ScheduledDiscount>
{
    public override void Configure(EntityTypeBuilder<ScheduledDiscount> builder)
    {
        base.Configure(builder);

        builder.Property(sd => sd.DiscountPercentage).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(sd => sd.FixedDiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(sd => sd.StartDate).IsRequired();
        builder.Property(sd => sd.EndDate).IsRequired();
        builder.Property(sd => sd.Name).HasMaxLength(100);

        builder.HasOne(sd => sd.Product)
            .WithMany(p => p.ScheduledDiscounts)
            .HasForeignKey(sd => sd.ProductId)
            .IsRequired();
    }
}
