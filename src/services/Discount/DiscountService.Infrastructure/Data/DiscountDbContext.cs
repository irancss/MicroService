using Microsoft.EntityFrameworkCore;
using DiscountService.Domain.Entities;
using DiscountService.Application.Interfaces;

namespace DiscountService.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Discount service
/// </summary>
public class DiscountDbContext : DbContext, IApplicationDbContext
{
    public DiscountDbContext(DbContextOptions<DiscountDbContext> options) : base(options)
    {
    }

    public DbSet<Discount> Discounts { get; set; }
    public DbSet<DiscountUsageHistory> DiscountUsageHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Discount entity
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CouponCode).HasMaxLength(50);
            entity.Property(e => e.Value).HasPrecision(18, 2);
            entity.Property(e => e.MinimumCartAmount).HasPrecision(18, 2);
            entity.Property(e => e.MaximumDiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);

            // Configure JSON columns for PostgreSQL
            entity.Property(e => e.ApplicableProductIds)
                .HasConversion(
                    v => v == null ? null : string.Join(',', v),
                    v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()
                );

            entity.Property(e => e.ApplicableCategoryIds)
                .HasConversion(
                    v => v == null ? null : string.Join(',', v),
                    v => v == null ? null : v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList()
                );

            // Create indexes for performance
            entity.HasIndex(e => e.CouponCode).IsUnique().HasFilter("\"CouponCode\" IS NOT NULL");
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsAutomatic);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.UserId).HasFilter("\"UserId\" IS NOT NULL");

            // Configure relationships
            entity.HasMany(e => e.UsageHistory)
                .WithOne(e => e.Discount)
                .HasForeignKey(e => e.DiscountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure DiscountUsageHistory entity
        modelBuilder.Entity<DiscountUsageHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.CartTotal).HasPrecision(18, 2);
            entity.Property(e => e.FinalTotal).HasPrecision(18, 2);
            entity.Property(e => e.CouponCode).HasMaxLength(50);
            entity.Property(e => e.UserEmail).IsRequired().HasMaxLength(255);

            // Create indexes for performance
            entity.HasIndex(e => e.DiscountId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.OrderId).IsUnique();
            entity.HasIndex(e => e.UsedAt);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for entities
        foreach (var entry in ChangeTracker.Entries<Discount>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
