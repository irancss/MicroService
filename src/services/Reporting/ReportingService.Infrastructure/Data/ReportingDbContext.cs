using Microsoft.EntityFrameworkCore;
using ReportingService.Domain.Entities;

namespace ReportingService.Infrastructure.Data;

public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
    {
    }

    // Fact Tables
    public DbSet<OrderFact> OrderFacts { get; set; }
    public DbSet<DailySalesAggregate> DailySalesAggregates { get; set; }

    // Dimension Tables
    public DbSet<DateDimension> DateDimensions { get; set; }
    public DbSet<CustomerDimension> CustomerDimensions { get; set; }
    public DbSet<ProductDimension> ProductDimensions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Schema configuration for data warehouse
        ConfigureFactTables(modelBuilder);
        ConfigureDimensionTables(modelBuilder);
        ConfigureIndexes(modelBuilder);
    }

    private static void ConfigureFactTables(ModelBuilder modelBuilder)
    {
        // Order Facts Table
        modelBuilder.Entity<OrderFact>(entity =>
        {
            entity.ToTable("order_facts", "reporting");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Revenue).HasPrecision(18, 2);
            entity.Property(e => e.Tax).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.Status).HasMaxLength(50);
            
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.OrderDate);
            entity.HasIndex(e => new { e.OrderDate, e.Currency });
        });

        // Daily Sales Aggregates Table
        modelBuilder.Entity<DailySalesAggregate>(entity =>
        {
            entity.ToTable("daily_sales_aggregates", "reporting");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TotalRevenue).HasPrecision(18, 2);
            entity.Property(e => e.TotalTax).HasPrecision(18, 2);
            entity.Property(e => e.TotalDiscount).HasPrecision(18, 2);
            entity.Property(e => e.AverageOrderValue).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            
            entity.HasIndex(e => new { e.Date, e.Currency }).IsUnique();
        });
    }

    private static void ConfigureDimensionTables(ModelBuilder modelBuilder)
    {
        // Date Dimension
        modelBuilder.Entity<DateDimension>(entity =>
        {
            entity.ToTable("date_dimensions", "reporting");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.DayOfWeek).HasMaxLength(20);
            entity.Property(e => e.MonthName).HasMaxLength(20);
            
            entity.HasIndex(e => e.Date).IsUnique();
            entity.HasIndex(e => e.Year);
            entity.HasIndex(e => e.Month);
            entity.HasIndex(e => e.Quarter);
        });

        // Customer Dimension
        modelBuilder.Entity<CustomerDimension>(entity =>
        {
            entity.ToTable("customer_dimensions", "reporting");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Segment).HasMaxLength(50);
            
            entity.HasIndex(e => e.CustomerId).IsUnique();
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Country);
            entity.HasIndex(e => e.Segment);
        });

        // Product Dimension
        modelBuilder.Entity<ProductDimension>(entity =>
        {
            entity.ToTable("product_dimensions", "reporting");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.SubCategory).HasMaxLength(100);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            
            entity.HasIndex(e => e.ProductId).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Brand);
            entity.HasIndex(e => e.IsActive);
        });
    }

    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Composite indexes for analytical queries
        modelBuilder.Entity<OrderFact>()
            .HasIndex(e => new { e.ProductDimensionId, e.CustomerDimensionId, e.DateDimensionId });
        
        modelBuilder.Entity<OrderFact>()
            .HasIndex(e => new { e.DateDimensionId, e.Currency, e.Status });
    }
}
