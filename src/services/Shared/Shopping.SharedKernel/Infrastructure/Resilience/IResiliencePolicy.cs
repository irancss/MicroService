namespace Shopping.SharedKernel.Infrastructure.Resilience;

public interface IRetryPolicy
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}

public interface ICircuitBreakerPolicy
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default);
    bool IsOpen { get; }
}

public class ResilienceOptions
{
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan TimeoutDuration { get; set; } = TimeSpan.FromSeconds(30);
}
