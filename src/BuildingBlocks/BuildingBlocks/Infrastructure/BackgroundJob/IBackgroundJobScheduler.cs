using System.Linq.Expressions;

namespace BuildingBlocks.Infrastructure.BackgroundJob;

/// <summary>
/// A generic interface for scheduling and enqueuing background jobs.
/// This abstracts away the specific implementation (e.g., Hangfire, Quartz.NET).
/// </summary>
public interface IBackgroundJobScheduler
{
    /// <summary>
    /// Schedules a fire-and-forget job to be executed at a later time.
    /// </summary>
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

    /// <summary>
    /// Enqueues a fire-and-forget job to be executed as soon as possible.
    /// </summary>
    string Enqueue<T>(Expression<Action<T>> methodCall);

    /// <summary>
    /// Enqueues a fire-and-forget job to be executed as soon as possible.
    /// </summary>
    string Enqueue(Expression<Action> methodCall);

    /// <summary>
    /// Creates or updates a recurring job.
    /// </summary>
    void AddOrUpdateRecurring<T>(string recurringJobId, Expression<Action<T>> methodCall, string cronExpression);

    /// <summary>
    /// Removes a recurring job.
    /// </summary>
    void RemoveRecurringJob(string recurringJobId);
}