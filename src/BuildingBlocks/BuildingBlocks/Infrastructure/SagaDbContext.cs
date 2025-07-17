// File: Infrastructure/SagaDbContext.cs

using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Messaging.EventDriven.Sagas;

public class SagaDbContext : SagaDbContext
{
    public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options) { }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new OrderStateMap(); }
    }
}

// این کلاس نحوه نگاشت OrderState به جدول دیتابیس را مشخص می‌کند
public class OrderStateMap : SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.OrderId).IsRequired();
        // Index گذاری روی فیلدهایی که برای جستجو استفاده می‌شوند
        entity.HasIndex(x => x.OrderId).IsUnique();
    }
}