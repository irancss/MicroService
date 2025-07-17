using Microsoft.EntityFrameworkCore;
using MarketingService.Domain.Entities;
using MarketingService.Domain.ValueObjects;

namespace MarketingService.Infrastructure.Data;

public class MarketingDbContext : DbContext
{
    public MarketingDbContext(DbContextOptions<MarketingDbContext> options) : base(options)
    {
    }

    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<LandingPage> LandingPages { get; set; }
    public DbSet<UserSegment> UserSegments { get; set; }
    public DbSet<UserSegmentMembership> UserSegmentMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Campaign configuration
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            
            // Value object configurations
            entity.OwnsOne(e => e.DateRange, dr =>
            {
                dr.Property(d => d.StartDate).HasColumnName("StartDate").IsRequired();
                dr.Property(d => d.EndDate).HasColumnName("EndDate").IsRequired();
            });

            entity.OwnsOne(e => e.Budget, b =>
            {
                b.Property(m => m.Amount).HasColumnName("BudgetAmount").HasPrecision(18, 2);
                b.Property(m => m.Currency).HasColumnName("BudgetCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.Metrics, m =>
            {
                m.Property(x => x.Impressions).HasColumnName("Impressions");
                m.Property(x => x.Clicks).HasColumnName("Clicks");
                m.Property(x => x.Conversions).HasColumnName("Conversions");
                m.OwnsOne(x => x.Spent, s =>
                {
                    s.Property(y => y.Amount).HasColumnName("SpentAmount").HasPrecision(18, 2);
                    s.Property(y => y.Currency).HasColumnName("SpentCurrency").HasMaxLength(3);
                });
                m.OwnsOne(x => x.Revenue, r =>
                {
                    r.Property(y => y.Amount).HasColumnName("RevenueAmount").HasPrecision(18, 2);
                    r.Property(y => y.Currency).HasColumnName("RevenueCurrency").HasMaxLength(3);
                });
            });

            // Convert list to JSON
            entity.Property(e => e.TargetSegmentIds)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>());
        });

        // LandingPage configuration
        modelBuilder.Entity<LandingPage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.MetaDescription).HasMaxLength(500);
            entity.Property(e => e.MetaKeywords).HasMaxLength(500);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
        });

        // UserSegment configuration
        modelBuilder.Entity<UserSegment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            
            // Convert criteria list to JSON
            entity.Property(e => e.Criteria)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<SegmentCriteria>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<SegmentCriteria>());

            entity.HasMany(e => e.Memberships)
                .WithOne(e => e.Segment)
                .HasForeignKey(e => e.SegmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserSegmentMembership configuration
        modelBuilder.Entity<UserSegmentMembership>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AssignedBy).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.UserId, e.SegmentId }).IsUnique();
        });
    }
}
