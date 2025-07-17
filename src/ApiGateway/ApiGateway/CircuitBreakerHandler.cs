namespace ApiGateway;
/// <summary>
/// Implements a circuit breaker pattern as an HTTP message handler.
/// This handler monitors requests and, if the number of failures exceeds a threshold,
/// it "breaks" the circuit, preventing further requests for a specified duration.
/// </summary>
public class CircuitBreakerHandler : DelegatingHandler
{
    private readonly int _eventsAllowed;
    private readonly TimeSpan _duration;
    private int _failures;
    private DateTime _blockedUntil = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircuitBreakerHandler"/> class.
    /// </summary>
    /// <param name="eventsAllowedBeforeBreaking">The number of consecutive failures allowed before the circuit breaks.</param>
    /// <param name="durationOfBreak">The duration for which the circuit will remain broken after it breaks.</param>
    public CircuitBreakerHandler(int eventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
    {
        _eventsAllowed = eventsAllowedBeforeBreaking;
        _duration = durationOfBreak;
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler.
    /// Implements the circuit breaker logic: if the circuit is broken, it throws an exception.
    /// Otherwise, it sends the request and updates the failure count based on the outcome.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message.</returns>
    /// <exception cref="Exception">Thrown when the circuit is currently broken.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (DateTime.UtcNow < _blockedUntil)
            throw new Exception("Circuit is currently broken");

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            _failures = 0; // Reset failures on success
            return response;
        }
        catch
        {
            _failures++;
            if (_failures >= _eventsAllowed)
            {
                _blockedUntil = DateTime.UtcNow.Add(_duration); // Break the circuit
            }
            throw; // Re-throw the exception
        }
    }
}

