using Hangfire;
using ProductService.Domain.Interfaces;

namespace ProductService.Infrastructure.Background
{
    public class HangfireRepository<T> : IHangfireRepository<T>, IDisposable
    {
        private readonly BackgroundJobServer _backgroundJobServer;

        public HangfireRepository()
        {
            // ایجاد و تنظیمات Hangfire
            GlobalConfiguration.Configuration.UseSqlServerStorage("connectionString"); // مثال: پیکربندی اتصال به SQL Server

            // ایجاد سرور Hangfire برای اجرای وظایف
            _backgroundJobServer = new BackgroundJobServer();
        }

        // متد برای ایجاد و زمان‌بندی وظیفه با استفاده از Hangfire با تاخیر مشخص
        public void ScheduleTask(Action<T> method, T parameter, TimeSpan delay)
        {
            BackgroundJob.Schedule(() => method(parameter), delay);
        }

        // متد برای ایجاد و زمان‌بندی وظیفه با استفاده از Hangfire با تاخیر مشخص و بازه زمانی تکرار
        public void ScheduleRecurringTask(Action<T> method, T parameter, string cronExpression)
        {
            RecurringJob.AddOrUpdate(() => method(parameter), cronExpression);
        }

        // متد برای اجرای وظیفه در زمان مشخص
        public void EnqueueTask(Action<T> method, T parameter)
        {
            BackgroundJob.Enqueue(() => method(parameter));
        }

        // متد برای اجرای وظیفه با تکرار در زمان‌های مشخص
        public void EnqueueRecurringTask(Action<T> method, T parameter, string cronExpression)
        {
            RecurringJob.AddOrUpdate(() => method(parameter), cronExpression);
        }

        // پیاده‌سازی IDisposable برای منابع رهایی
        public void Dispose()
        {
            _backgroundJobServer.Dispose();
        }
    }
}
