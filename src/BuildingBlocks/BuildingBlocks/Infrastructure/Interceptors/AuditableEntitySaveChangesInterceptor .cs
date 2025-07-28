using BuildingBlocks.Core.Contracts;
using BuildingBlocks.Domain.Entities; // [اصلاح شد] استفاده از AuditableEntity صحیح
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Interceptors
{
    /// <summary>
    /// [اصلاح شد] این Interceptor اکنون به درستی فیلدهای AuditableEntity را قبل از ذخیره در دیتابیس پر می‌کند.
    /// </summary>
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTime;

        public AuditableEntitySaveChangesInterceptor(
            IDateTimeProvider dateTime,
            ICurrentUserService currentUserService)
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

            var userId = _currentUserService.UserId;
            var now = _dateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedAt = now;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedAt = now;
                }

                if (entry.State == EntityState.Deleted)
                {
                    // [نکته] پیاده‌سازی Soft Delete
                    // اگر می‌خواهید از Soft Delete استفاده کنید، state را به Modified تغییر دهید.
                    // این کار نیاز به تنظیم یک Query Filter سراسری برای IsDeleted == false دارد.
                    // entry.State = EntityState.Modified;
                    // entry.Entity.IsDeleted = true;
                    // entry.Entity.UpdatedBy = userId;
                    // entry.Entity.UpdatedAt = now;
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