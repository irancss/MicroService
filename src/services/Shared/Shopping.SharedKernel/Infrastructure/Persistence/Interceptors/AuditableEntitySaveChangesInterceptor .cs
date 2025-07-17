// File: SharedKernel/Persistence/Interceptors/AuditableEntitySaveChangesInterceptor.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shopping.SharedKernel.Core.Contracts;
using Shopping.SharedKernel.Domain.Entities;

namespace Shopping.SharedKernel.Infrastructure.Persistence.Interceptors // <-- تغییر کرد
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService? _currentUserService; // <-- به صورت اختیاری اضافه شد
        private readonly IDateTime _dateTime;

        // کانستراکتور را برای دریافت هر دو سرویس آپدیت میکنیم
        public AuditableEntitySaveChangesInterceptor(
            IDateTime dateTime,
            ICurrentUserService? currentUserService = null) // <-- به صورت nullable که اجباری نباشد
        {
            _dateTime = dateTime;
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            // دریافت نام کاربر جاری (در صورت وجود سرویس)
            var userId = _currentUserService?.UserId;

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    // entry.Entity.CreatedBy = userId; // <-- اگر فیلدش را دارید
                    entry.Entity.CreatedAt = _dateTime.Now;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    // entry.Entity.UpdatedBy = userId; // <-- اگر فیلدش را دارید
                    entry.Entity.UpdatedAt = _dateTime.Now;
                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}