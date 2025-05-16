using System.Diagnostics;

namespace ApiGateway
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingHandler> _logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Start: {request.Method} {request.RequestUri}");

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                _logger.LogInformation($"Completed: {request.Method} {request.RequestUri} " +
                                       $"=> {response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed: {request.Method} {request.RequestUri}");
                throw;
            }
        }
    }
}
