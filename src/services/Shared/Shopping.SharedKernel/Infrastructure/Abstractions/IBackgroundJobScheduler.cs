namespace Shopping.SharedKernel.Domain;

public interface IBackgroundJobScheduler // A better, more generic name
{
    void Schedule<T>(Action<T> method, TimeSpan delay);
    void Enqueue<T>(Action<T> method);
    void AddOrUpdateRecurring<T>(string recurringJobId, Action<T> method, string cronExpression);
    // ... etc
}