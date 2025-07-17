namespace ProductService.Domain.Interfaces
{
    public interface IHangfireRepository<T>
    {
        // متد برای ایجاد و زمان‌بندی وظیفه با استفاده از Hangfire با تاخیر مشخص
        void ScheduleTask(Action<T> method, T parameter, TimeSpan delay);

        // متد برای ایجاد و زمان‌بندی وظیفه با استفاده از Hangfire با تاخیر مشخص و بازه زمانی تکرار
        void ScheduleRecurringTask(Action<T> method, T parameter, string cronExpression);

        // متد برای اجرای وظیفه در زمان مشخص
        void EnqueueTask(Action<T> method, T parameter);

        // متد برای اجرای وظیفه با تکرار در زمان‌های مشخص
        void EnqueueRecurringTask(Action<T> method, T parameter, string cronExpression);
    }
}
