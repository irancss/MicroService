using Microsoft.EntityFrameworkCore;
using VendorService.Domain.Models;

namespace VendorService.Infrastructure.Data.Configurations;

public class VendorDbContext : DbContext
{
    public VendorDbContext(DbContextOptions<VendorDbContext> options) : base(options)
    {
    }

    public DbSet<Vendor> Vendors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
                // Default configuration for PostgreSQL
        modelBuilder.HasDefaultSchema("public");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendorDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}