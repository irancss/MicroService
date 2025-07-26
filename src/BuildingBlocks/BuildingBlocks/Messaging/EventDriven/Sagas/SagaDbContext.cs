using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    /// <summary>
    /// [اصلاح شد] این DbContext اکنون عمومی است و هیچ وابستگی به State های خاص ندارد.
    /// MassTransit به صورت خودکار mapping ها را مدیریت می‌کند.
    /// </summary>
    public class SagaDbContext : DbContext
    {
        public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // [اصلاح شد] به جای ثبت تک به تک، تمام پیکربندی‌های Saga از این اسمبلی
            // به صورت خودکار پیدا و اعمال می‌شوند.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SagaDbContext).Assembly);

        }
    }

    // [جدید] پیکربندی صریح برای OrderState با استفاده از IEntityTypeConfiguration
    public class OrderStateEntityTypeConfiguration : IEntityTypeConfiguration<OrderState>
    {
        public void Configure(EntityTypeBuilder<OrderState> builder)
        {
            // MassTransit به طور خودکار فیلدهای اصلی مانند CorrelationId و CurrentState را map می‌کند.
            // در اینجا می‌توانید پیکربندی‌های اضافی مانند ایندکس‌ها را اضافه کنید.
            builder.Property(s => s.CurrentState).HasMaxLength(64).IsRequired();
            builder.HasIndex(s => s.OrderId).IsUnique();
        }
    }

    // [جدید] پیکربندی صریح برای CustomerOnboardingState با استفاده از IEntityTypeConfiguration
    public class CustomerOnboardingStateEntityTypeConfiguration : IEntityTypeConfiguration<CustomerOnboardingState>
    {
        public void Configure(EntityTypeBuilder<CustomerOnboardingState> builder)
        {
            builder.Property(s => s.CurrentState).HasMaxLength(64).IsRequired();
            builder.Property(s => s.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(s => s.Email);
        }
    }
}