namespace ApiGateway
{
    public class CircuitBreakerHandler : DelegatingHandler
    {
        private readonly int _eventsAllowed;
        private readonly TimeSpan _duration;
        private int _failures;
        private DateTime _blockedUntil = DateTime.MinValue;

        public CircuitBreakerHandler(int eventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            _eventsAllowed = eventsAllowedBeforeBreaking;
            _duration = durationOfBreak;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (DateTime.UtcNow < _blockedUntil)
                throw new Exception("Circuit is currently broken");

            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                _failures = 0;
                return response;
            }
            catch
            {
                _failures++;
                if (_failures >= _eventsAllowed)
                    _blockedUntil = DateTime.UtcNow.Add(_duration);
                throw;
            }
        }
    }
}
