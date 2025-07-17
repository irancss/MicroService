using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Domain;

public class MediaInfoEntityTypeConfiguration : IEntityTypeConfiguration<MediaInfo>
{
    public void Configure(EntityTypeBuilder<MediaInfo> builder)
    {
        // This configuration is for a generic MediaInfo entity.
        // If MediaInfo is an owned type or base class, configuration would be different.
        builder.ToTable("MediaInfos"); // If it's a standalone table
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Url).IsRequired().HasMaxLength(1024);
        builder.Property(m => m.MediaType).IsRequired().HasMaxLength(50); // Or .HasConversion<string>() if MediaType is an enum
        builder.Property(m => m.AltText).HasMaxLength(255);
        builder.Property(m => m.Order).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.Property(m => m.FileSize).HasColumnType("text"); // In bytes

        builder.Property(m => m.CreatedAt).IsRequired();
        // Assuming MediaInfo might be linked to various entities, or used by ProductImage
    }
}