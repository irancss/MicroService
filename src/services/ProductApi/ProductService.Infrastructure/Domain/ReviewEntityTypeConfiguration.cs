using BuildingBlocks.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain
{
    public class ReviewEntityTypeConfiguration : BaseEntityTypeConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);

            builder.Property(r => r.UserId).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(2000);
            builder.Property(r => r.IsApproved).IsRequired();
            builder.Property(r => r.Title).HasMaxLength(100);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .IsRequired();
        }
    }
}
