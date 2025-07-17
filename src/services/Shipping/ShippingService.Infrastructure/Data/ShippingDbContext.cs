using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Domain;
using ShippingService.Domain.Entities;
using ShippingService.Infrastructure.Configuration;
using ShippingService.Infrastructure.Data.Configurations;

namespace ShippingService.Infrastructure.Data;

public class ShippingDbContext : DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
    {
    }

    public DbSet<ShippingMethod> ShippingMethods { get; set; }
    public DbSet<TimeSlotTemplate> TimeSlotTemplates { get; set; }
    public DbSet<TimeSlotBooking> TimeSlotBookings { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentTracking> ShipmentTrackings { get; set; }
    public DbSet<ShipmentReturn> ShipmentReturns { get; set; }
    public DbSet<ReturnTracking> ReturnTrackings { get; set; }
    public DbSet<PremiumSubscription> PremiumSubscriptions { get; set; }
    public DbSet<SubscriptionUsageLog> SubscriptionUsageLogs { get; set; }
    public DbSet<FreeShippingRule> FreeShippingRules { get; set; }
    public DbSet<FreeShippingCondition> FreeShippingConditions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new ShippingMethodConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotTemplateConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotBookingConfiguration());
        
        // Set schema to shipping
        modelBuilder.HasDefaultSchema("shipping");

        modelBuilder.ApplyConfiguration(new ShipmentConfiguration());
        modelBuilder.ApplyConfiguration(new ShipmentTrackingConfiguration());
        modelBuilder.ApplyConfiguration(new ShipmentReturnConfiguration());
        modelBuilder.ApplyConfiguration(new ReturnTrackingConfiguration());
        
        // تنظیم نگاشت‌های جدید
        modelBuilder.ApplyConfiguration(new PremiumSubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionUsageLogConfiguration());
        modelBuilder.ApplyConfiguration(new FreeShippingRuleConfiguration());
        modelBuilder.ApplyConfiguration(new FreeShippingConditionConfiguration());
    }
}
