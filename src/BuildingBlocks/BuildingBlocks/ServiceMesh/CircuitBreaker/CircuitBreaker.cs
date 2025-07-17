using Microsoft.Extensions.Logging;

namespace BuildingBlocks.ServiceMesh.CircuitBreaker
{
    public interface ICircuitBreaker
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> operation);
        Task ExecuteAsync(Func<Task> operation);
        CircuitBreakerState State { get; }
    }

    public interface ICircuitBreakerFactory
    {
        ICircuitBreaker CreateCircuitBreaker(string name, CircuitBreakerOptions? options = null);
    }

    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }

    public class CircuitBreakerOptions
    {
        public int FailureThreshold { get; set; } = 5;
        public TimeSpan OpenTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public int SuccessThreshold { get; set; } = 1;
    }

    public class CircuitBreaker : ICircuitBreaker
    {
        private readonly CircuitBreakerOptions _options;
        private readonly ILogger<CircuitBreaker> _logger;
        private readonly object _lock = new object();
        
        private CircuitBreakerState _state = CircuitBreakerState.Closed;
        private int _failureCount = 0;
        private int _successCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;

        public CircuitBreakerState State
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }

        public CircuitBreaker(CircuitBreakerOptions options, ILogger<CircuitBreaker> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            await CheckStateAsync();

            try
            {
                var result = await operation();
                await OnSuccessAsync();
                return result;
            }
            catch (Exception ex)
            {
                await OnFailureAsync(ex);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            await CheckStateAsync();

            try
            {
                await operation();
                await OnSuccessAsync();
            }
            catch (Exception ex)
            {
                await OnFailureAsync(ex);
                throw;
            }
        }

        private Task CheckStateAsync()
        {
            lock (_lock)
            {
                if (_state == CircuitBreakerState.Open)
                {
                    if (DateTime.UtcNow - _lastFailureTime >= _options.OpenTimeout)
                    {
                        _state = CircuitBreakerState.HalfOpen;
                        _successCount = 0;
                        _logger.LogInformation("Circuit breaker transitioned to Half-Open state");
                    }
                    else
                    {
                        throw new CircuitBreakerException("Circuit breaker is open");
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSuccessAsync()
        {
            lock (_lock)
            {
                _failureCount = 0;

                if (_state == CircuitBreakerState.HalfOpen)
                {
                    _successCount++;
                    if (_successCount >= _options.SuccessThreshold)
                    {
                        _state = CircuitBreakerState.Closed;
                        _logger.LogInformation("Circuit breaker transitioned to Closed state");
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task OnFailureAsync(Exception exception)
        {
            lock (_lock)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                if (_state == CircuitBreakerState.HalfOpen)
                {
                    _state = CircuitBreakerState.Open;
                    _logger.LogWarning("Circuit breaker transitioned to Open state from Half-Open due to failure: {Exception}", exception.Message);
                }
                else if (_failureCount >= _options.FailureThreshold)
                {
                    _state = CircuitBreakerState.Open;
                    _logger.LogWarning("Circuit breaker transitioned to Open state due to {FailureCount} failures", _failureCount);
                }
            }

            return Task.CompletedTask;
        }
    }

    public class CircuitBreakerFactory : ICircuitBreakerFactory
    {
        private readonly ILogger<CircuitBreaker> _logger;
        private readonly Dictionary<string, ICircuitBreaker> _circuitBreakers = new();

        public CircuitBreakerFactory(ILogger<CircuitBreaker> logger)
        {
            _logger = logger;
        }

        public ICircuitBreaker CreateCircuitBreaker(string name, CircuitBreakerOptions? options = null)
        {
            if (_circuitBreakers.ContainsKey(name))
            {
                return _circuitBreakers[name];
            }

            var circuitBreaker = new CircuitBreaker(options ?? new CircuitBreakerOptions(), _logger);
            _circuitBreakers[name] = circuitBreaker;
            return circuitBreaker;
        }
    }

    public class CircuitBreakerException : Exception
    {
        public CircuitBreakerException(string message) : base(message)
        {
        }
    }
}
